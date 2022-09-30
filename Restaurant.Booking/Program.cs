using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;

namespace Restaurant.Booking;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddMassTransit(x =>
                {
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

                services.AddOptions<MassTransitHostOptions>()
                    .Configure(options =>
                    {
                        options.WaitUntilStarted = true;
                    });

                services.AddTransient<Restaurant>();

                services.AddHostedService<Worker>();
            });
}