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
    private async Task SimulateProcessing(TimeSpan interval, CancellationToken stoppingToken = default)
    {
        do
        {
            var range = (7, 15);
            var randomGuestArrivalInterval = TimeSpan.FromSeconds(new Random().Next(range.Item1, range.Item2 + 1));

            Console.WriteLine($"Запрос на бронь. Время прибыти: {randomGuestArrivalInterval}.");

            var bookingRequested = new BookingRequested(NewId.NextGuid(), 
                                                        NewId.NextGuid(), 
                                                        DateTime.UtcNow, 
                                                        randomGuestArrivalInterval);

            await _bus.Publish<IBookingRequested>(bookingRequested, stoppingToken);

            await Task.Delay(interval, stoppingToken);

        } while (!stoppingToken.IsCancellationRequested);
    }
}