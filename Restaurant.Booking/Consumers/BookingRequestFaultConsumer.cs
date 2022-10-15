using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking;

public sealed class BookingRequestedFaultConsumer : IConsumer<Fault<IBookingRequested>>
{
    public Task Consume(ConsumeContext<Fault<IBookingRequested>> context)
    {
        Console.WriteLine($"[OrderId: {context.Message.Message.OrderId}] Отмена в зале");
        return Task.CompletedTask;
    }
}