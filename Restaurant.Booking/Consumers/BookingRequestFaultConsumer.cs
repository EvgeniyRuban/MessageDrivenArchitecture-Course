using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

public sealed class BookingRequestedFaultConsumer : IConsumer<Fault<IBookingRequested>>
{
    private readonly Restaurant _restaurant;

    public BookingRequestedFaultConsumer(Restaurant restaurant)
    {
        ArgumentNullException.ThrowIfNull(restaurant, nameof(restaurant));
        _restaurant = restaurant;
    }

    public Task Consume(ConsumeContext<Fault<IBookingRequested>> context)
    {
        Console.WriteLine($"[Order: {context.Message.Message.OrderId}] - отмена в зале.");

        return Task.CompletedTask;
    }
}