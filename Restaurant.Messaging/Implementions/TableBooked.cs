namespace Restaurant.Messaging;

public class TableBooked : ITableBooked
{
    public TableBooked(Guid orderId, Guid clientId, DateTime creationDate, bool isSuccess = true)
    {
        OrderId = orderId;
        ClientId = clientId;
        CreationDate = creationDate;
        IsSuccess = isSuccess;
    }

    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public bool IsSuccess { get; }
    public DateTime CreationDate { get; }
}