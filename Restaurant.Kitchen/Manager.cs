using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Kitchen;

public sealed class Manager
{
    private readonly IBus _bus;

    public Manager(IBus bus)
    {
        ArgumentNullException.ThrowIfNull(bus, nameof(bus));
        _bus = bus;
    }

    public void CheckKitchenReady(Guid orderId, Dish? dish)
    {
        _bus.Publish<IKitchenReady>(new KitchenReady(orderId, true));
    }
}