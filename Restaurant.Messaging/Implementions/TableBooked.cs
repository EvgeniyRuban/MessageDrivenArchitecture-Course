namespace Restaurant.Messaging;

public class TableBooked : ITableBooked
{
    public TableBooked(Guid orderId, 
                       Guid clientId, 
                       Guid tableId,
                       DateTime creationDate, 
                       bool isSuccess = true)
    {
        OrderId = orderId;
        ClientId = clientId;
        TableId = tableId;
        CreationDate = creationDate;
        IsSuccess = isSuccess;
    }

    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Guid TableId { get; }
    public bool IsSuccess { get; }
    public DateTime CreationDate { get; }
}