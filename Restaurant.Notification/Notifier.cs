using System.Collections.Concurrent;

namespace Restaurant.Notification;

public sealed class Notifier
{
    private readonly ConcurrentDictionary<Guid, Tuple<Guid?, Accepted>> _states;
    private readonly object _lock = new ();

    public Notifier()
    {
        _states = new();
    }

    public void Accept(Guid orderId, Accepted accepted, Guid? clientId = null)
    {
        _states.AddOrUpdate(orderId, new Tuple<Guid?, Accepted>(clientId, accepted),
                (guid, oldValue) => new Tuple<Guid?, Accepted>(
                    oldValue.Item1 ?? clientId, oldValue.Item2 | accepted));
    }
    public void Notify(Guid orderId)
    {
        lock (_lock)
        {
            var booking = _states[orderId];

            switch (booking.Item2)
            {
                case Accepted.All:
                    Console.WriteLine($"Успешно забронировано для клиента {booking.Item1}");
                    _states.Remove(orderId, out _);
                    break;
                case Accepted.Rejected:
                    Console.WriteLine($"Гость {booking.Item1}, к сожалению, все столики заняты");
                    _states.Remove(orderId, out _);
                    break;
                case Accepted.Kitchen:
                case Accepted.Booking:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    public void ResetCache()
    {
        _states.Clear();
        Console.Clear();
        Console.WriteLine("К сожалению, в связи с техническими проблемами на кухне, все брони предзаказы нам пришлось отменить.\n" +
                          "Приносим свои извинения.");
    }
    public void ResetCache(Guid orderId) => _states.TryRemove(orderId, out _);
}