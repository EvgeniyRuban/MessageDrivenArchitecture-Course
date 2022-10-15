namespace Restaurant.Messaging;

public sealed class BookingCancellation : IBookingCancellation
{
    public BookingCancellation(Guid orderId)
    {
        ArgumentNullException.ThrowIfNull(orderId, nameof(orderId));

        OrderId = orderId;
    }

    public Guid OrderId { get; }
}