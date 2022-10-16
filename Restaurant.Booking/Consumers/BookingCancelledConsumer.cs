using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

public sealed class BookingCancelledConsumer : IConsumer<IBookingCancelled>
{
    private readonly Restaurant _restaurant;

    public BookingCancelledConsumer(Restaurant restaurant)
    {
        _restaurant = restaurant;
    }

    public async Task Consume(ConsumeContext<IBookingCancelled> context)
    {
        var result = await _restaurant.UnbookTableAsync(context.Message.TableId);
    }
}