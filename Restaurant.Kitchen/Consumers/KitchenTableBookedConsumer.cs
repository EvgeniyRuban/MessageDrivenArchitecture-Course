using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Kitchen.Consumers;

public sealed class KitchenTableBookedConsumer : IConsumer<ITableBooked>
{
    private readonly Kitchen _kitchen;

    public KitchenTableBookedConsumer(Kitchen kitchen)
    {
        ArgumentNullException.ThrowIfNull(kitchen, nameof(kitchen));

        _kitchen = kitchen;
    }

    public Task Consume(ConsumeContext<ITableBooked> context)
    {
        var result = context.Message.IsSucces;

        if (result)
        {
            _kitchen.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder);
        }

        return context.ConsumeCompleted;
    }
}