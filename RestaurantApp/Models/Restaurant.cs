namespace RestaurantApp;

public sealed class Restaurant
{
    private readonly List<Table> _tables;

    public Restaurant()
    {
        _tables = GetRandomTables(10);
    }

    public void BookTable(int numberOfSeats)
    {
        Console.WriteLine("Добрый день! Подождите секунду я подберу столик и подтвержу вашу бронь, оставайтесь на линии.");

        var table = _tables.FirstOrDefault(t => 
                                           t.NumberOfSeats >= numberOfSeats &&
                                           t.State == TableState.Free);
        Task.Delay(5000).Wait();

        if(table is null)
        {
            Console.WriteLine("К сожалению, сейчас все столики заняты.");
            return;
        }

        Console.WriteLine($"Готово! Ваш столик номер {table.Id}");
    }
    public async Task BookTableAsync(int numberOfSeats)
    {
        Console.WriteLine("Добрый день! Подождите секунду я подберу столик и подтвержу вашу бронь, оставайтесь на линии.");

        Task.Run(async () =>
        {
            var table = _tables.FirstOrDefault(t =>
                                           t.NumberOfSeats >= numberOfSeats &&
                                           t.State == TableState.Free);
            Task.Delay(5000).Wait();

            if (table is null)
            {
                Console.WriteLine("К сожалению, сейчас все столики заняты.");
                return;
            }

            Console.WriteLine($"Готово! Ваш столик номер {table.Id}");
        });
    }
    public void UnbookTable(int id)
    {
        var table = _tables.FirstOrDefault(t => t.Id == id);

        if (table is null)
        {
            Console.WriteLine("");
        }
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
                    i,
                    arr[rnd.Next(arr.Length)]
                ));
        }

        return tables;
    }
}