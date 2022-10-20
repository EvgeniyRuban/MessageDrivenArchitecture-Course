using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

internal sealed class BookingRequestedConsumer : IConsumer<IBookingRequested>
{
    private readonly IProcessedMessagesRepository _processedMessagesRepository;
    private readonly Restaurant _restaurant;

    public BookingRequestedConsumer(Restaurant restaurant, IProcessedMessagesRepository processedMessagesRepository)
    {
        ArgumentNullException.ThrowIfNull(restaurant, nameof(restaurant));
        ArgumentNullException.ThrowIfNull(processedMessagesRepository, nameof(processedMessagesRepository));

        _restaurant = restaurant;
        _processedMessagesRepository = processedMessagesRepository;
    }

    public async Task Consume(ConsumeContext<IBookingRequested> context)
    {
        var message = new ProcessedMessage()
        {
            OrderId = context.Message.OrderId,
            MessageId = (Guid)context.MessageId,
        };

        if (await _processedMessagesRepository.Contain(message))
        {
            return;
        }

        await _processedMessagesRepository.Add(message);

        var bookedTableId = await _restaurant.BookTableAsync(new Random().Next((int)NumberOfSeats.Twelve + 1));

        if (bookedTableId is not null)
        {
            await context.Publish<ITableBooked>(new TableBooked(context.Message.OrderId, 
                                                                context.Message.ClientId,
                                                                (Guid)bookedTableId,
                                                                context.Message.CreationDate,
                                                                bookedTableId is not null));
        }

        string answer = bookedTableId is null
            ? "неудалось найти подходящий столик."
            : $"для вас подобран столик.";

        Console.WriteLine($"[Order: {context.Message.OrderId}] - {answer}");
    }
}