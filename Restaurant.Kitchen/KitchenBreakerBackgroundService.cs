using MassTransit;
using Microsoft.Extensions.Hosting;
using Restaurant.Messaging;

namespace Restaurant.Kitchen;

internal class KitchenBreakerBackgroundService : BackgroundService
{
    private readonly IBus _bus;

    public KitchenBreakerBackgroundService(Kitchen kitchen, IBus bus)
    {
        ArgumentNullException.ThrowIfNull(bus, nameof(bus));
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(20));
        int chanceToBreak = 10;

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            if(chanceToBreak > new Random().Next(100))
            {
                await _bus.Publish<KitchenBroken>(new(), stoppingToken);
                Console.Clear();
                Console.WriteLine("Поломка!!!");
            }
        }

        return;
    }
}