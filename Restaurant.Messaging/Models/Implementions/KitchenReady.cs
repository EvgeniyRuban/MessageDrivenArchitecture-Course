namespace Restaurant.Messaging;

public class KitchenReady : IKitchenReady
{
    public KitchenReady(Guid orderId, bool isReady)
    {
        OrderId = orderId;
        IsReady = isReady;
    }

    public Guid OrderId { get; }
    public bool IsReady { get; }
}