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
        Console.WriteLine($"[OrderId: {context.Message.Message.OrderId}] Отмена в зале");

        return Task.CompletedTask;
    }
}