using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

internal sealed class TableBookedFaultConsumer : IConsumer<Fault<ITableBooked>>
{
    public Task Consume(ConsumeContext<Fault<ITableBooked>> context)
    {
        return Task.CompletedTask;
    }
}
