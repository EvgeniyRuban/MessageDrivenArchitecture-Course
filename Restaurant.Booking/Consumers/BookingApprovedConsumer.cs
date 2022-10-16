using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

public sealed class BookingApprovedConsumer : IConsumer<IBookingApproved>
{
    public async Task Consume(ConsumeContext<IBookingApproved> context)
    {
        var range = (7, 15);
        var interval = TimeSpan.FromSeconds(new Random().Next(range.Item1, range.Item2 + 1));

        await Task.Delay(interval);

        Console.WriteLine($"[Order {context.Message.OrderId}] - гость прибыл.");

        context.Publish<IGuestArrived>(new GuestArrived(context.Message.OrderId));
    }
}