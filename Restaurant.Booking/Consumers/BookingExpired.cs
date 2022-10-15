using Restaurant.Messaging;

namespace Restaurant.Booking;

public sealed class BookingExpired : IBookingExpired
{
    private readonly RestaurantBooking _instance;

    public BookingExpired(RestaurantBooking instance)
    {
        _instance = instance;
    }

    public Guid OrderId => _instance.OrderId;
}