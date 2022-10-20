namespace Restaurant.Booking;

public interface IInMemoryRepository<T> where T : class
{
    void Add(T entity);
    IReadOnlyCollection<T> GetAll();
}