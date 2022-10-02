namespace Restaurant.Booking;

public sealed class Restaurant
{
    private readonly IList<Table> _tables;
    private readonly TimeSpan _syncOperationDelay = TimeSpan.FromSeconds(5);

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

        table = _tables.FirstOrDefault(t =>
                                           t.SeatsCount >= numberOfSeats &&
                                           t.State == TableState.Free);

        Task.Delay(_syncOperationDelay).Wait();

        if (table is null)
        {
            return null;
        }

        table.SetState(TableState.Booked);

        return table.Id;
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
            var table = _tables.FirstOrDefault(t =>
                                           t.SeatsCount >= numberOfSeats &&
                                           t.State == TableState.Free);

            if (table is null)
            {
                return null;
            }

            table.SetState(TableState.Booked);

            return table.Id;
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

        table = _tables.FirstOrDefault(t => t.Id == id);

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
            var table = _tables.FirstOrDefault(t => t.Id == id);

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
        });
    }
    private List<Table> GetRandomTables(int count)
    {
        var tables = new List<Table>(count);
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
            tables.Add(
                new(
                   i + 1,
                   (int)arr[rnd.Next(arr.Length)]));
        }

        return tables;
    }
}