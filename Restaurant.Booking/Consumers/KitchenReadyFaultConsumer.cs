using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

internal sealed class KitchenReadyFaultConsumer : IConsumer<Fault<IKitchenReady>>
{
    public Task Consume(ConsumeContext<Fault<IKitchenReady>> context)
    {
        return Task.CompletedTask;
    }
}