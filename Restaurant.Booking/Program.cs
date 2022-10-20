using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Restaurant.Booking.Consumers;

namespace Restaurant.Booking;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = GetConfigurationSection<AppSettings>()
            [AppSettingsKeys.ConsoleTitle];

        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var rabbitMqConfig = GetConfigurationSection<RabbitMqHostSettings>();
        var connectionStringConfig = GetConfigurationSection<SqlServerSettings>();

        var connectionStringComponents = new SqlServerConnectionStringComponents()
        {
            Server = connectionStringConfig[SqlServerConnectionComponentsKeys.Server],
            Database = connectionStringConfig[SqlServerConnectionComponentsKeys.Database],
            UserId = connectionStringConfig[SqlServerConnectionComponentsKeys.UserId],
            Password = connectionStringConfig[SqlServerConnectionComponentsKeys.Password],
        };

        var builder = 
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<RestaurantBookingDbContext>(options =>
                {
                    options.UseSqlServer(connectionStringComponents.Combine());
                });

                services.AddMassTransit(x =>
                {
                    x.AddConsumer<BookingRequestedConsumer>().Endpoint(config => config.Temporary = true);
                    x.AddConsumer<BookingApprovedConsumer>().Endpoint(config => config.Temporary = true);
                    x.AddConsumer<BookingFaultedConsumer>().Endpoint(config => config.Temporary = true);

                    x.AddSagaStateMachine<BookingStateMachine, BookingState>()
                            .Endpoint(e => e.Temporary = true)
                            .InMemoryRepository();

                    x.AddDelayedMessageScheduler();

                    x.UsingRabbitMq((context, config) =>
                    {
                        config.Host(
                            host: rabbitMqConfig[RabbitMqHostSettingsKeys.Host],
                            virtualHost: rabbitMqConfig[RabbitMqHostSettingsKeys.VirtualHost],
                            hostSettings =>
                            {
                                hostSettings.Username(rabbitMqConfig[RabbitMqHostSettingsKeys.User]);
                                hostSettings.Password(rabbitMqConfig[RabbitMqHostSettingsKeys.Password]);
                            });

                        config.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(10),
                                                                       TimeSpan.FromMinutes(20),
                                                                       TimeSpan.FromMinutes(30)));

                        config.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3)));

                        config.UseDelayedMessageScheduler();
                        config.UseInMemoryOutbox();
                        config.ConfigureEndpoints(context);
                    });
                });

                services.AddSingleton<Restaurant>();
                services.AddTransient<BookingState>();
                services.AddTransient<BookingStateMachine>();
                services.AddScoped<IProcessedMessagesRepository, ProcessedMessagesRepository>();

                services.AddHostedService<WorkerBackgroundService>();
            });

        return builder;
    }

    private static IConfigurationRoot? GetConfigurationSection<T>() where T : class
        => new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<T>()
                .Build();
}