using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Notification.Consumers;

public sealed class NotifierTableBookedConsumer : IConsumer<ITableBooked>
{
    private readonly Notifier _notifier;

    public NotifierTableBookedConsumer(Notifier notifier)
    {
        _notifier = notifier;
    }

    public Task Consume(ConsumeContext<ITableBooked> context)
    {
        var result = context.Message.IsSucces;

        _notifier.Accept(context.Message.OrderId,
                         result ? Accepted.Booking : Accepted.Rejected,
                         context.Message.ClientId);

        return Task.CompletedTask;
    }
}