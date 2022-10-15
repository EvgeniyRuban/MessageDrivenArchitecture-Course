namespace Restaurant.Messaging;

public interface IBookingCancellation
{
    public Guid OrderId { get; }
}