using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking;

public sealed class RestaurantBookingRequestConsumer : IConsumer<IBookingRequested>
{
    private readonly Restaurant _restaurant;

    public RestaurantBookingRequestConsumer(Restaurant restaurant)
    {
        ArgumentNullException.ThrowIfNull(restaurant, nameof(restaurant));

        _restaurant = restaurant;
    }

    public async Task Consume(ConsumeContext<IBookingRequested> context)
    {
        Console.WriteLine($"[OrderId: {context.Message.OrderId}]");
        var result = await _restaurant.BookTableAsync(1);

        await context.Publish<TableBooked>(new (context.Message.OrderId, context.Message.ClientId, result is not null));
    }
}