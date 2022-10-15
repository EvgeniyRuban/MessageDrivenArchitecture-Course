namespace Restaurant.Messaging;

public sealed class BookingRequested : IBookingRequested
{
    public BookingRequested(Guid orderId, Guid clientId, DateTime creationDate, Dish[]? preorder = null)
    {
        OrderId = orderId;
        ClientId = clientId;
        CreationDate = creationDate;
        PreOrder = preorder;
    }

    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish[]? PreOrder { get; }
    public DateTime CreationDate { get; }
}