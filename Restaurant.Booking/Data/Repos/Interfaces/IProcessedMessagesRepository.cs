namespace Restaurant.Booking;

public interface IProcessedMessagesRepository
{
    Task<bool> Contain(ProcessedMessage message);
    Task Add(ProcessedMessage message);
    Task Delete(ProcessedMessage message);
}