namespace Restaurant.Messaging;

public sealed class BookingRequest : IBookingRequested
{
    public BookingRequest(Guid orderId, Guid clientId, DateTime creationDate, Dish[]? preorder = null)
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