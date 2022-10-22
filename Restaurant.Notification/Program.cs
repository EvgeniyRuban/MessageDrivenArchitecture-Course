using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Restaurant.Notification.Consumers;
using MassTransit.Audit;

namespace Restaurant.Notification;

internal class Program
{
    private static IConfiguration Configuration { get; set; }

    private static void Main(string[] args)
    {
        Configuration = BuildConfiguration();

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>().ConsoleTitle;

        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var rabbitMqConfig = Configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();

        var builder = 
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddSingleton<IMessageAuditStore, AuditStore>();
                    var auditStore = services.BuildServiceProvider().GetService<IMessageAuditStore>();

                    x.AddConsumer<NotifyConsumer>().Endpoint(cfg => cfg.Temporary = true);

                    x.UsingRabbitMq((context, config) =>
                    {
                        config.Host(
                            host: rabbitMqConfig.Host,
                            virtualHost: rabbitMqConfig.VirtualHost,
                            hostSettings =>
                            {
                                hostSettings.Username(rabbitMqConfig.User);
                                hostSettings.Password(rabbitMqConfig.Password);
                            });

                        config.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(10),
                                                                       TimeSpan.FromMinutes(20),
                                                                       TimeSpan.FromMinutes(30)));

                        config.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3)));

                        config.UseInMemoryOutbox();
                        config.ConfigureEndpoints(context);

                        config.ConnectSendAuditObservers(auditStore);
                        config.ConnectConsumeAuditObserver(auditStore);
                    });
                });

                services.AddTransient<Notifier>();
            });

        return builder;
    }

    private static IConfiguration BuildConfiguration()
        => new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .Build();
}