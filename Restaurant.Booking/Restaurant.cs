using System.Collections.Concurrent;

namespace Restaurant.Booking;

public sealed class Restaurant
{
    private readonly ConcurrentDictionary<int, Table> _tables;
    private readonly TimeSpan _syncOperationDelay = TimeSpan.FromSeconds(5);
    private readonly object _lock = new ();

    public Restaurant()
    {
        _tables = GetRandomTables(10);
    }

    /// <summary>
    /// The table booking.
    /// </summary>
    /// <param name="numberOfSeats"></param>
    /// <returns> 
    /// Returns a table id, if restaurant contains table with current <paramref name = "numberOfSeats"/>, otherwise null.
    /// </returns>
    public int? BookTable(int numberOfSeats)
    {
        Table? table = null;

        lock (_lock)
        {
            table = _tables.FirstOrDefault(pair =>
                                           pair.Value.SeatsCount >= numberOfSeats &&
                                           pair.Value.State == TableState.Free).Value;

            Task.Delay(_syncOperationDelay).Wait();

            if (table is null)
            {
                return null;
            }

            table.SetState(TableState.Booked);

            return table.Id;
        }
    }
    /// <summary>
    /// The table booking asynchronously.
    /// </summary>
    /// <param name="numberOfSeats"></param>
    /// <returns> 
    /// Returns a table id, if restaurant contains table with current <paramref name = "numberOfSeats"/>, otherwise null.
    /// </returns>
    public async Task<int?> BookTableAsync(int numberOfSeats)
    {
        return await Task.Run<int?>(() =>
        {
            lock (_lock)
            {
                var table = _tables.FirstOrDefault(pair =>
                                               pair.Value.SeatsCount >= numberOfSeats &&
                                               pair.Value.State == TableState.Free).Value;

                if (table is null)
                {
                    return null;
                }

                table.SetState(TableState.Booked);

                return table.Id;
            }
        });
    }
    /// <summary>
    /// The table unbooking.
    /// </summary>
    /// <param name="id">Table id.</param>
    /// <returns>
    /// Returns true if unbooking operation completed succesfully, if table with current <paramref name="id"/>
    /// is free, returns false, otherwise null (table with current <paramref name="id"/> doesn't exists).
    /// </returns>
    public bool? UnbookTable(int id)
    {
        Table? table = null;

        lock (_lock)
        {
            table = _tables.FirstOrDefault(pair => pair.Value.Id == id).Value;

            Task.Delay(_syncOperationDelay).Wait();

            if (table is null)
            {
                return null;
            }
            else if (table.State == TableState.Free)
            {
                return false;
            }

            table?.SetState(TableState.Free);

            return true;
        }
    }
    /// <summary>
    /// The table unbooking asynchronously.
    /// </summary>
    /// <param name="id">Table id.</param>
    /// <returns>
    /// Returns true if unbooking operation completed succesfully, if table with current <paramref name="id"/>
    /// is free, returns false, otherwise null (table with current <paramref name="id"/> doesn't exists).
    /// </returns>
    public async Task<bool?> UnbookTableAsync(int id)
    {
        return await Task.Run<bool?>(() =>
        {
            lock (_lock)
            {
                var table = _tables.FirstOrDefault(pair => pair.Value.Id == id).Value;

                if (table is null)
                {
                    return null;
                }
                else if (table.State == TableState.Free)
                {
                    return false;
                }

                table?.SetState(TableState.Free);

                return true;
            }
        });
    }
    public async Task UnbookTables()
    {
        await Task.Run(() =>
        {
            lock (_lock)
            {
                foreach (var table in _tables.Values)
                {
                    if (table.State == TableState.Booked)
                    {
                        table.SetState(TableState.Free);
                    }
                }
            }
        });
    }
    public Task SetBookingAutoCancellation(int tableId, CancellationToken stoppingToken = default)
    {
        return Task.Run(() =>
        {
            stoppingToken.ThrowIfCancellationRequested();
            new Timer((_) => UnbookTable(tableId),
                  null,
                  TimeSpan.FromSeconds(20),
                  default);
        }, stoppingToken);
    }
    private ConcurrentDictionary<int, Table> GetRandomTables(int count)
    {
        var tables = new ConcurrentDictionary<int, Table>();
        var rnd = new Random();
        NumberOfSeats[] arr =
        {
                NumberOfSeats.Single,
                NumberOfSeats.Double,
                NumberOfSeats.Four,
                NumberOfSeats.Six,
                NumberOfSeats.Eight,
                NumberOfSeats.Ten,
                NumberOfSeats.Twelve,
        };

        for (int i = 0; i < count; i++)
        {
            tables.TryAdd(
                i + 1, 
                new (i + 1,(int)arr[rnd.Next(arr.Length)]));
        }

        return tables;
    }
}