using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

public sealed class BookingApprovedConsumer : IConsumer<IBookingApproved>
{
    public async Task Consume(ConsumeContext<IBookingApproved> context)
    {
        var range = (5, 15);
        var randomGuestArrivalInterval = TimeSpan.FromSeconds(new Random().Next(range.Item1, range.Item2 + 1));

        await Task.Delay(randomGuestArrivalInterval);

        context.Publish<IGuestArrived>(new GuestArrived(context.Message.OrderId));
    }
}