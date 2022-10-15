using Microsoft.Extensions.Hosting;
using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking;

public sealed class RestaurantWorkerBackgroundService : BackgroundService
{
    private readonly IBus _bus;
    private readonly Restaurant _restaurant;

    public RestaurantWorkerBackgroundService(IBus bus, Restaurant restaurant)
    {
        ArgumentNullException.ThrowIfNull(bus, nameof(bus));
        ArgumentNullException.ThrowIfNull(restaurant, nameof(restaurant));

        _bus = bus;
        _restaurant = restaurant;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        await SimulateProcessing(TimeSpan.FromSeconds(10), stoppingToken);
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
            Console.WriteLine("Привет! Желаете забронировать столик?");

            var bookingRequested = new BookingRequested(NewId.NextGuid(), NewId.NextGuid(), DateTime.UtcNow);

            await _bus.Publish<IBookingRequested>(bookingRequested, stoppingToken);

            await Task.Delay(interval, stoppingToken);

        } while (!stoppingToken.IsCancellationRequested);
    }
}