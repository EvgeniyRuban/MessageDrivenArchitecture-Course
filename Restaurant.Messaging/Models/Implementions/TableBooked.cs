namespace Restaurant.Messaging;

public class TableBooked : ITableBooked
{
    public TableBooked(Guid orderId, Guid clientId, bool succes, Dish? preOrder = null)
    {
        OrderId = orderId;
        ClientId = clientId;
        Succes = succes;
        PreOrder = preOrder;
    }

    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish? PreOrder { get; }
    public bool Succes { get; }
}