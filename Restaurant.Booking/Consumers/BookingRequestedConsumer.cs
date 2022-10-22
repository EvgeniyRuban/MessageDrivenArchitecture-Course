using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

internal sealed class BookingRequestedConsumer : IConsumer<IBookingRequested>
{
    private readonly Restaurant _restaurant;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger _logger;

    public BookingRequestedConsumer(Restaurant restaurant, 
                                    IServiceScopeFactory serviceScopeFactory,
                                    ILogger<BookingRequestedConsumer> logger)
    {
        ArgumentNullException.ThrowIfNull(restaurant, nameof(restaurant));
        ArgumentNullException.ThrowIfNull(serviceScopeFactory, nameof(serviceScopeFactory));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _serviceScopeFactory = serviceScopeFactory;
        _restaurant = restaurant;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IBookingRequested> context)
    {
        var processedMessagesrepository = _serviceScopeFactory.CreateScope()
            .ServiceProvider.GetRequiredService<IProcessedMessagesRepository>();

        var message = new ProcessedMessage()
        {
            OrderId = context.Message.OrderId,
            MessageId = (Guid)context.MessageId,
        };

        if (await processedMessagesrepository.Contain(message))
        {
            return;
        }

        await processedMessagesrepository.Add(message);
        _logger.LogInformation("Processed message removing, scheduled", message);
        ScheduleProcessedMessageRemoving(message, TimeSpan.FromSeconds(30));

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

    public async Task ScheduleProcessedMessageRemoving(ProcessedMessage message, TimeSpan deleteAfter)
    {
        await Task.Delay(deleteAfter);
        var repository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IProcessedMessagesRepository>();
        await repository.Delete(message);
        _logger.LogInformation("Processed message removing, completed.", message);
    }
}