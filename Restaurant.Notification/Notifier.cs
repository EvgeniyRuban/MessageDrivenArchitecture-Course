namespace Restaurant.Notification;

public sealed class Notifier
{
    public void Notify(Guid orderId, string message)
    {
        Console.WriteLine($"[OrderID: {orderId}] Уважаемый клиент! {message}.");
    }
}