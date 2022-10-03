namespace Restaurant.Messaging;

public interface ITableBooked
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish[]? PreOrder { get; }
    public bool IsSucces { get; }
}