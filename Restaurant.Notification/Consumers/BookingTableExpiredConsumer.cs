using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Notification.Consumers;

public sealed class BookingTableExpiredConsumer : IConsumer<IBookingTableExpired>
{
    private readonly Notifier _notifier;

    public BookingTableExpiredConsumer(Notifier notifier)
    {
        ArgumentNullException.ThrowIfNull(notifier, nameof(notifier));

        _notifier = notifier;
    }

    public Task Consume(ConsumeContext<IBookingTableExpired> context)
    {
        _notifier.ResetCache(context.Message.OrderId);

        Console.WriteLine($"Заказ {context.Message.OrderId} был автоматически аннулирован.");

        return context.ConsumeCompleted;
    }
}