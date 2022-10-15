namespace Restaurant.Messaging;

public interface ITableBooked
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public bool IsSuccess { get; }
    public DateTime CreationDate { get; }
}