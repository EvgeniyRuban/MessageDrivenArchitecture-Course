namespace Restaurant.Messaging;

public sealed class BookingCancelled : IBookingCancelled
{
    public BookingCancelled(Guid orderId, Guid tableId)
    {
        ArgumentNullException.ThrowIfNull(orderId, nameof(orderId));

        OrderId = orderId;
    }

    public Guid OrderId { get; }
    public Guid TableId { get; }
}