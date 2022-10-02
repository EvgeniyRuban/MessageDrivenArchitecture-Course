using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Kitchen.Consumers;

public sealed class KitchenTableBookedConsumer : IConsumer<ITableBooked>
{
    private readonly Manager _manager;

    public KitchenTableBookedConsumer(Manager manager)
    {
        ArgumentNullException.ThrowIfNull(manager, nameof(manager));

        _manager = manager;
    }

    public Task Consume(ConsumeContext<ITableBooked> context)
    {
        var result = context.Message.IsSucces;

        if (result)
        {
            _manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder);
        }

        return context.ConsumeCompleted;
    }
}