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
        await SimulateProcessing(TimeSpan.FromSeconds(10), stoppingToken);
    }

    /// <summary>
    /// Run user booking request simulation.
    /// </summary>
    /// <param name="interval">Interval between requests.</param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    private async Task SimulateProcessing(TimeSpan interval, CancellationToken stoppingToken = default)
    {
        var preorder = new Dish[]{ Dish.Lasagna, Dish.Pizza, Dish.Pasta, Dish.Chicken };
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        do
        {
            Console.WriteLine("Привет! Желаете забронировать столик?");

            var bookedTableId = await _restaurant.BookTableAsync(new Random().Next((int)NumberOfSeats.Twelve + 1));

            var bookingCancellationTask = _restaurant.SetBookingAutoCancellation((int)bookedTableId, stoppingToken);

            var tableBooked = new TableBooked(NewId.NextGuid(), NewId.NextGuid(), bookedTableId is not null, preorder);

            PublishByEnding(bookingCancellationTask, new(tableBooked.OrderId, tableBooked.ClientId));

            await _bus.Publish(tableBooked, stoppingToken);

            await Task.Delay(interval, stoppingToken);
        } while (!stoppingToken.IsCancellationRequested);
    }

    private async Task PublishByEnding(Task task, BookingTableExpired data)
    {
        ArgumentNullException.ThrowIfNull(task, nameof(task));

        await task;

        _bus.Publish(data);
    }
}