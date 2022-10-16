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
        Console.Title = GetConfigurationSection<AppSettings>()
            [$"{nameof(AppSettings)}:{AppSettingsDefinition.ConsoleTitle}"];

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var config = GetConfigurationSection<RabbitMQHostConfig>();

        var builder =
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<KitchenBookingRequestedConsumer>()
                        .Endpoint(cfg => cfg.Temporary = true);

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

                services.AddSingleton<Kitchen>();
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