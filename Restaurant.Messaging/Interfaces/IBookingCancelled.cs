namespace Restaurant.Messaging;

public interface IBookingCancelled
{
    public Guid OrderId { get; }
}