namespace Restaurant.Booking;

internal interface IProcessedMessagesRepository
{
    Task<bool> Contain(ProcessedMessage message);
    Task Add(ProcessedMessage message);
    Task Delete(Guid orderId);
}