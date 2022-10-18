using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MassTransit;
using Restaurant.Kitchen.Consumers;

namespace Restaurant.Kitchen;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = GetConfigurationSection<AppSettings>()[AppSettingsKeys.ConsoleTitle];

        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var rabbitMqConfig = GetConfigurationSection<RabbitMqHostSettings>();

        var builder =
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<KitchenBookingRequestedConsumer>().Endpoint(cfg => cfg.Temporary = true);

                    x.AddDelayedMessageScheduler();

                    x.UsingRabbitMq((context, config) =>
                    {
                        config.Host(host: rabbitMqConfig[RabbitMqHostSettingsKeys.Host],
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

                services.AddSingleton<Kitchen>();
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