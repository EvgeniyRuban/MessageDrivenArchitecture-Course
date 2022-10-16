using Microsoft.Extensions.Hosting;
using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking;

public sealed class WorkerBackgroundService : BackgroundService
{
    private readonly IBus _bus;

    public WorkerBackgroundService(IBus bus)
    {
        ArgumentNullException.ThrowIfNull(bus, nameof(bus));
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        await SimulateProcessing(TimeSpan.FromSeconds(20), stoppingToken);
    }

    /// <summary>
    /// Run user booking request simulation.
    /// </summary>
    /// <param name="interval">Interval between requests.</param>
    /// <returns></returns>
    private async Task SimulateProcessing(TimeSpan retryInterval, CancellationToken stoppingToken = default)
    {
        do
        {
            var arrivalVia = TimeSpan.FromSeconds(10);
            Console.WriteLine($"\nЗапрос на бронь. Прибытие через: {arrivalVia}.");

            var bookingRequested = new BookingRequested(NewId.NextGuid(), 
                                                        NewId.NextGuid(), 
                                                        DateTime.UtcNow, 
                                                        arrivalVia);

            await _bus.Publish<IBookingRequested>(bookingRequested, stoppingToken);

            await Task.Delay(retryInterval, stoppingToken);

        } while (!stoppingToken.IsCancellationRequested);
    }
}