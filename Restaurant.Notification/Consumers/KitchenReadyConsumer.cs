using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Notification.Consumers;

public class KitchenReadyConsumer : IConsumer<IKitchenReady>
{
    private readonly Notifier _notifier;

    public KitchenReadyConsumer(Notifier notifier)
    {
        _notifier = notifier;
    }

    public Task Consume(ConsumeContext<IKitchenReady> context)
    {
        var kitchenIsReady = context.Message.IsReady;

        _notifier.Accept(context.Message.OrderId, 
                        kitchenIsReady ? Accepted.Kitchen
                                       : Accepted.Rejected);

        _notifier.Notify(context.Message.OrderId);

        return Task.CompletedTask;
    }
}