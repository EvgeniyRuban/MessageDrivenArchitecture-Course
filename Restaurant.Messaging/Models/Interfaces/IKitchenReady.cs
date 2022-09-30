namespace Restaurant.Messaging;

public interface IKitchenReady
{
    public Guid OrderId { get; }
    public bool IsReady { get; }
}