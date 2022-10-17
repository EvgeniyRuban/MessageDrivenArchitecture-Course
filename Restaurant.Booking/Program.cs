using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
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
        var rabbitMqConfig = GetConfigurationSection<RabbitMqHostConfig>();

        var builder = 
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer(GetGenericConsumerConfigurator<BookingRequestedConsumer>())
                        .Endpoint(cfg => cfg.Temporary = true);

                    x.AddConsumer(GetGenericConsumerConfigurator<BookingApprovedConsumer>())
                        .Endpoint(cfg => cfg.Temporary = true);

                    x.AddConsumer(GetGenericConsumerConfigurator<BookingCancelledConsumer>())
                        .Endpoint(cfg => cfg.Temporary = true);

                    x.AddConsumer(GetGenericConsumerConfigurator<BookingRequestedFaultConsumer>())
                        .Endpoint(cfg => cfg.Temporary = true);

                    x.AddConsumer(GetGenericConsumerConfigurator<KitchenReadyFaultConsumer>())
                        .Endpoint(cfg => cfg.Temporary = true);

                    x.AddConsumer(GetGenericConsumerConfigurator<TableBookedFaultConsumer>())
                        .Endpoint(cfg => cfg.Temporary = true);

                    x.AddSagaStateMachine<BookingStateMachine, BookingState>()
                            .Endpoint(e => e.Temporary = true)
                            .InMemoryRepository();

                    x.AddDelayedMessageScheduler();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(
                            host: rabbitMqConfig[RabbitMqHostConfigKeys.Host],
                            virtualHost: rabbitMqConfig[RabbitMqHostConfigKeys.VirtualHost],
                            hostSettings =>
                            {
                                hostSettings.Username(rabbitMqConfig[RabbitMqHostConfigKeys.User]);
                                hostSettings.Password(rabbitMqConfig[RabbitMqHostConfigKeys.Password]);
                            });

                        cfg.UseDelayedMessageScheduler();
                        cfg.UseInMemoryOutbox();
                        cfg.ConfigureEndpoints(context);
                    });
                });

                services.AddSingleton<Restaurant>();
                services.AddTransient<BookingState>();
                services.AddTransient<BookingStateMachine>();

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

    private static Action<IConsumerConfigurator<TConsumer>> GetGenericConsumerConfigurator<TConsumer>() 
        where TConsumer : class
        => config =>
        {
            config.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(10),
                                                           TimeSpan.FromSeconds(20),
                                                           TimeSpan.FromSeconds(30)));

            config.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3)));
        };
}