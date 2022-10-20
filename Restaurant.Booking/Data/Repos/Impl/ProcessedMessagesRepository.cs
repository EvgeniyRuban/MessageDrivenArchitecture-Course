using Microsoft.EntityFrameworkCore;

namespace Restaurant.Booking;

internal class ProcessedMessagesRepository : IProcessedMessagesRepository
{
    private readonly RestaurantBookingDbContext _dbContext;

    public ProcessedMessagesRepository(RestaurantBookingDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        _dbContext = dbContext;
    }

    public async Task Add(ProcessedMessage message)
    {
        await _dbContext.ProcessedMessages.AddAsync(message);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<bool> Contain(ProcessedMessage message)
        => await _dbContext.ProcessedMessages.FirstOrDefaultAsync(m => m.OrderId == message.OrderId
                                                                    && m.MessageId == message.MessageId) is not null;
    public async Task Delete(Guid orderId)
    {
        var result = await _dbContext.ProcessedMessages.FirstOrDefaultAsync(m => m.OrderId == orderId);
        if (result is not null)
        {
            _dbContext.ProcessedMessages.Remove(result);
            await _dbContext.SaveChangesAsync();
        }
    }
}