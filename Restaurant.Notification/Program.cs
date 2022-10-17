using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Restaurant.Notification.Consumers;

namespace Restaurant.Notification;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = GetConfigurationSection<AppSettings>()
            [AppSettingsKeys.ConsoleTitle];

        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var config = GetConfigurationSection<RabbitMqHostSettings>();

        var builder = 
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<NotifyConsumer>()
                        .Endpoint(cfg => cfg.Temporary = true);

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(
                            host: config[RabbitMqHostSettingsKeys.Host],
                            virtualHost: config[RabbitMqHostSettingsKeys.VirtualHost],
                            hostSettings =>
                            {
                                hostSettings.Username(config[RabbitMqHostSettingsKeys.User]);
                                hostSettings.Password(config[RabbitMqHostSettingsKeys.Password]);
                            });

                        cfg.ConfigureEndpoints(context);
                    });
                });

                services.AddTransient<Notifier>();
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