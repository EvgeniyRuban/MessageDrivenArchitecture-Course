namespace Restaurant.Messaging;

public sealed class BookingTableExpired : IBookingTableExpired
{
    public BookingTableExpired(Guid orderId, Guid clientId)
    {
        OrderId = orderId;
        ClientId = clientId;
    }

    public Guid OrderId { get; }
    public Guid ClientId { get; }
}