using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

public sealed class KitchenBrokenConsumer : IConsumer<IKitchenBroken>
{
    private readonly Restaurant _restaurant;

    public KitchenBrokenConsumer(Restaurant restaurant)
    {
        _restaurant = restaurant;
    }

    public async Task Consume(ConsumeContext<IKitchenBroken> context)
    {
        await _restaurant.UnbookTables();
        Console.Clear();
        Console.WriteLine("По причине поломки на кухне все брони аннулированы.");
        return;
    }
}