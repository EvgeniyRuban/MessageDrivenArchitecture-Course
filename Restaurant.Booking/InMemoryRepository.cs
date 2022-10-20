using System.Collections.Concurrent;

namespace Restaurant.Booking;

public sealed class InMemoryRepository<T> : IInMemoryRepository<T> where T : class
{
    private readonly ConcurrentBag<T> _holder;

    public InMemoryRepository()
    {
        _holder = new ();
    }

    public void Add(T entity) => _holder.Add(entity);
    public IReadOnlyCollection<T> GetAll() => _holder;
}