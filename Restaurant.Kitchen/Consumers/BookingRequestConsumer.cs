using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Kitchen;

public sealed class BookingRequestedConsumer : IConsumer<IBookingRequested>
{
    private readonly Kitchen _kitchen;

    public BookingRequestedConsumer(Kitchen kitchen)
    {
        ArgumentNullException.ThrowIfNull(kitchen, nameof(kitchen));

        _kitchen = kitchen;
    }

    public Task Consume(ConsumeContext<IBookingRequested> context)
    {
        var delay = new Random().Next(1, 10);
        Task.Delay(TimeSpan.FromSeconds(delay)).Wait();

        _kitchen.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder);

        return context.ConsumeCompleted;
    }
}