using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

internal sealed class BookingRequestedFaultConsumer : IConsumer<Fault<IBookingRequested>>
{
    public Task Consume(ConsumeContext<Fault<IBookingRequested>> context)
    {
        return Task.CompletedTask;
    }
}