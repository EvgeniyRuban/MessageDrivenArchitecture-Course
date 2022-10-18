using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Kitchen.Consumers;

internal sealed class KitchenBookingRequestedConsumer : IConsumer<IBookingRequested>
{
    private readonly Kitchen _kitchen;

    public KitchenBookingRequestedConsumer(Kitchen kitchen)
    {
        ArgumentNullException.ThrowIfNull(kitchen, nameof(kitchen));

        _kitchen = kitchen;
    }

    public Task Consume(ConsumeContext<IBookingRequested> context)
    {
        if (!_kitchen.CheckKitchenReady(context.Message.PreOrder))
        {
            Console.WriteLine($"[Order: {context.Message.OrderId}] - отмена, по стоп листу.");
        }
        else
        {
            context.Publish<IKitchenReady>(new KitchenReady(context.Message.OrderId));
            Console.WriteLine($"[Order: {context.Message.OrderId}] - кухня готова выполнить заказ.");
        }

        return context.ConsumeCompleted;
    }
}