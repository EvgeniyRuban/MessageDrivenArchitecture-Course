namespace Restaurant.Messaging;

public interface IBookingCancelled
{
    public Guid OrderId { get; }
    public Guid TableId { get; }
}