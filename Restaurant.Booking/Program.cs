using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MassTransit;

namespace Restaurant.Booking;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = GetConfigurationSection<AppSettings>()
            [$"{nameof(AppSettings)}:{AppSettingsDefinition.ConsoleTitle}"];

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var config = GetConfigurationSection<RabbitMQHostConfig>();

        var builder = 
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<KitchenBrokenConsumer>()
                     .Endpoint(cfg => cfg.Temporary = true);

                    x.AddConsumer<BookingRequestedFaultConsumer>()
                     .Endpoint(cfg => cfg.Temporary = true);

                    x.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>()
                            .Endpoint(e => e.Temporary = true)
                            .InMemoryRepository();

                    x.AddDelayedMessageScheduler();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(
                            host: config[$"{nameof(RabbitMQHostConfig)}:{RabbitMQHostConfigDefinition.HostName}"],
                            virtualHost: config[$"{nameof(RabbitMQHostConfig)}:{RabbitMQHostConfigDefinition.VirtualHost}"],
                            hostSettings =>
                            {
                                hostSettings.Username(config[$"{nameof(RabbitMQHostConfig)}:{RabbitMQHostConfigDefinition.UserName}"]);
                                hostSettings.Password(config[$"{nameof(RabbitMQHostConfig)}:{RabbitMQHostConfigDefinition.Password}"]);
                            });

                        cfg.UseDelayedMessageScheduler();
                        cfg.UseInMemoryOutbox();
                        cfg.ConfigureEndpoints(context);
                    });
                });

                services.AddOptions<MassTransitHostOptions>()
                    .Configure(options =>
                    {
                        options.WaitUntilStarted = true;
                    });

                services.AddSingleton<Restaurant>();

                services.AddHostedService<RestaurantWorkerBackgroundService>();
            });

        return builder;
    }

    public static IConfigurationRoot? GetConfigurationSection<T>() where T : class
        => new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<T>()
                .Build();
}