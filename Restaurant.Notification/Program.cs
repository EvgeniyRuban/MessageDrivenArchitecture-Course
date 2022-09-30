using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Notification.Consumers;

namespace Restaurant.Notification;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumers(typeof(NotifierTableBookedConsumer).Assembly);

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("shrimp-01.rmq.cloudamqp.com", "sfbzerjl",
                            settings =>
                            {
                                settings.Username("sfbzerjl");
                                settings.Password("7sdVW8O46lm2XW98UUt1-V3Ia2VjMkry");
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
}