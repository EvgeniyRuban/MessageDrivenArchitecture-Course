namespace Restaurant.Messaging;

public class TableBooked : ITableBooked
{
    public TableBooked(Guid orderId, Guid clientId, bool isSucces, Dish? preOrder = null)
    {
        OrderId = orderId;
        ClientId = clientId;
        IsSucces = isSucces;
        PreOrder = preOrder;
    }

    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish? PreOrder { get; }
    public bool IsSucces { get; }
}