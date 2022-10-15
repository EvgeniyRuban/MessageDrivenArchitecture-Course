using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

public sealed class BookingRequestedConsumer : IConsumer<IBookingRequested>
{
    private readonly Restaurant _restaurant;

    public BookingRequestedConsumer(Restaurant restaurant)
    {
        ArgumentNullException.ThrowIfNull(restaurant, nameof(restaurant));

        _restaurant = restaurant;
    }

    public async Task Consume(ConsumeContext<IBookingRequested> context)
    {
        var bookedTableId = await _restaurant.BookTableAsync(new Random().Next((int)NumberOfSeats.Twelve + 1));

        if (bookedTableId is not null)
        {
            await context.Publish<ITableBooked>(
                new TableBooked(
                    context.Message.OrderId,
                    context.Message.ClientId,
                    context.Message.CreationDate,
                    bookedTableId is not null));
        }

        string answer = bookedTableId is null
            ? "неудалось найти подходящий столик."
            : $"для вас подобран столик номер {bookedTableId}.";

        Console.WriteLine($"[OrderId: {context.Message.OrderId}] - {answer}");
    }
}