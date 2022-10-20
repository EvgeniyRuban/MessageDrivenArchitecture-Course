using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

internal sealed class BookingRequestedConsumer : IConsumer<IBookingRequested>
{
    private readonly Restaurant _restaurant;
    private readonly IInMemoryRepository<BookingRequestedModel> _repository;

    public BookingRequestedConsumer(Restaurant restaurant, IInMemoryRepository<BookingRequestedModel> repositiry)
    {
        ArgumentNullException.ThrowIfNull(restaurant, nameof(restaurant));
        ArgumentNullException.ThrowIfNull(repositiry, nameof(repositiry));

        _restaurant = restaurant;
        _repository = repositiry;
    }

    public async Task Consume(ConsumeContext<IBookingRequested> context)
    {
        var model = _repository.GetAll().FirstOrDefault(i => i.OrderId == context.Message.OrderId);

        if(model is not null && model.CheckMessageId(context.MessageId.ToString()))
        {
            Console.WriteLine($"[OrderId: {context.Message.OrderId}] - dublicate message sending from bookingrequested consumer was catched.");
            return;
            
        }

        _repository.Add(new(context.Message.OrderId,
                            context.Message.ClientId,
                            context.Message.ArriveVia,
                            context.Message.CreationDate,
                            context.MessageId.ToString()));

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