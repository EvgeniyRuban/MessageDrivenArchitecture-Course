using System.Text;
using Messaging;
using Microsoft.Extensions.Hosting;

namespace Restaurant.Notification;

public class Startup : BackgroundService
{
    private readonly Consumer _consumer;

    public Startup()
    {
        _consumer = new (new ConsumerConfig
        {
            UserName = "sfbzerjl",
            Password = "7sdVW8O46lm2XW98UUt1-V3Ia2VjMkry",
            HostName = "shrimp-01.rmq.cloudamqp.com",
            VirtualHost = "sfbzerjl",
            QueueName = "BookingNotification",
            RoutingKey = String.Empty,
            Exchange = "fanout_exchange",
            Port = 5672,
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Receive((sender, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" Received [x]: {message}");
        });
    }
}