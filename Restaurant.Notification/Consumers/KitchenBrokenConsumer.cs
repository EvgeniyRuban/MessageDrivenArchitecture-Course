using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Notification.Consumers;

public sealed class KitchenBrokenConsumer : IConsumer<IKitchenBroken>
{
    private readonly Notifier _notifier;

    public KitchenBrokenConsumer(Notifier notifier)
    {
        _notifier = notifier;
    }

    public Task Consume(ConsumeContext<IKitchenBroken> context)
    {
        _notifier.ResetCache();

        return context.ConsumeCompleted;
    }
}