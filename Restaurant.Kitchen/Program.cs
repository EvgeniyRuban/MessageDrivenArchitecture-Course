using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Restaurant.Kitchen.Consumers;

namespace Restaurant.Kitchen;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<KitchenTableBookedConsumer>();

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

                    services.AddSingleton<Manager>();

                    services.AddOptions<MassTransitHostOptions>()
                    .Configure(options =>
                    {
                        options.WaitUntilStarted = true;
                    });
                });
}