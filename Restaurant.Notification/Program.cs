using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Notification.Consumers;

namespace Restaurant.Notification;

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
                    x.AddConsumers(typeof(NotifierTableBookedConsumer).Assembly);

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
                    });
                });
                services.AddSingleton<Notifier>();

                services.AddOptions<MassTransitHostOptions>()
                    .Configure(options =>
                    {
                        options.WaitUntilStarted = true;
                    });
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