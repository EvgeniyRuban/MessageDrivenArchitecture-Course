namespace RestaurantApp;

public sealed class Restaurant
{
    private readonly List<Table> _tables;
    private readonly TimeSpan _syncOperationDelay = TimeSpan.FromSeconds(5);

    public Restaurant()
    {
        _tables = GetRandomTables(10);
    }

    public void BookTable(int numberOfSeats)
    {
        Console.WriteLine("Подождите секунду я подберу столик и подтвержу вашу бронь, оставайтесь на линии.");

        var table = _tables.FirstOrDefault(t => 
                                           t.SeatsCount >= numberOfSeats &&
                                           t.State == TableState.Free);

        Task.Delay(_syncOperationDelay).Wait();

        if(table is null)
        {
            Console.WriteLine("К сожалению, сейчас все столики заняты.");
            return;
        }

        table.SetState(TableState.Booked);
        Console.WriteLine($"Готово! Ваш столик номер {table.Id}");
    }
    public async Task BookTableAsync(int numberOfSeats)
    {
        Task.Run(async () =>
        {
            var table = _tables.FirstOrDefault(t =>
                                           t.SeatsCount >= numberOfSeats &&
                                           t.State == TableState.Free);
            if (table is null)
            {
                Console.WriteLine("К сожалению, сейчас все столики заняты.");
                return;
            }

            table.SetState(TableState.Booked);
            Console.WriteLine($"Готово! Ваш столик номер {table.Id}");
        });
    }
    public void UnbookTable(int id)
    {
        var table = _tables.FirstOrDefault(t => t.Id == id);

        Task.Delay(_syncOperationDelay).Wait();

        if (table is null)
        {
            Console.WriteLine("У нас нет стола с указанным номером.");
            return;
        }
        else if(table.State == TableState.Free)
        {
            Console.WriteLine("Стол с указанным номером не занят.");
            return;
        }

        table?.SetState(TableState.Free);
        Console.WriteLine("Бронь снята.");
    }
    public async Task UnbookTableAsync(int id)
    {
        Task.Run(async () =>
        {
            var table = _tables.FirstOrDefault(t => t.Id == id);

            if (table is null)
            {
                Console.WriteLine("У нас нет стола с указанным номером.");
                return;
            }
            else if (table.State == TableState.Free)
            {
                Console.WriteLine("Стол с указанным номером не занят.");
                return;
            }

            table?.SetState(TableState.Free);
            Console.WriteLine("Бронь снята.");
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
                new
                (
                    i + 1,
                    (int)arr[rnd.Next(arr.Length)]
                ));
        }

        return tables;
    }
}