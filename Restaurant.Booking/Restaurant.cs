using Restaurant.Notification;

namespace Restaurant.Booking;

public sealed class Restaurant
{
    private readonly List<Table> _tables;
    private readonly ClientNotifier _notifier;
    private readonly object _syncObj = new object();
    private readonly TimeSpan _syncOperationDelay = TimeSpan.FromSeconds(5);

    public Restaurant()
    {
        _notifier = new ClientNotifier();
        _tables = GetRandomTables(10);
    }

    public void BookTable(int numberOfSeats)
    {
        Table table = null!;
        Console.WriteLine("Подождите секунду я подберу столик и подтвержу вашу бронь, оставайтесь на линии.");

        lock (_syncObj)
        {
            table = _tables.FirstOrDefault(t =>
                                               t.SeatsCount >= numberOfSeats &&
                                               t.State == TableState.Free);
        }

        Task.Delay(_syncOperationDelay).Wait();

        lock (_syncObj)
        {
            if (table is null)
            {
                Console.WriteLine("К сожалению, сейчас все столики заняты.");
                return;
            }

            table.SetState(TableState.Booked);
            Console.WriteLine($"Готово! Ваш столик номер {table.Id}");
        }
    }
    public async Task BookTableAsync(int numberOfSeats)
    {
        Task.Run(async () =>
        {
            lock (_syncObj)
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

                _notifier.SendAsync($"Готово! Ваш столик номер {table.Id}");
            }
        });
    }
    public void UnbookTable(int id)
    {
        Table table = null!;

        lock (_syncObj)
        {
            table = _tables.FirstOrDefault(t => t.Id == id);
        }

        Task.Delay(_syncOperationDelay).Wait();

        lock (_syncObj)
        {
            if (table is null)
            {
                Console.WriteLine($"У нас нет стола с номером {id}.");
                return;
            }
            else if (table.State == TableState.Free)
            {
                Console.WriteLine($"Стол с номером {id}, не занят.");
                return;
            }

            table?.SetState(TableState.Free);
        }

        Console.WriteLine($"Бронь {id} стола снята.");
    }
    public async Task UnbookTableAsync(int id)
    {
        Task.Run(async () =>
        {
            lock (_syncObj)
            {
                var table = _tables.FirstOrDefault(t => t.Id == id);

                if (table is null)
                {
                    _notifier.SendAsync($"У нас нет стола с номером {id}.");
                    return;
                }
                else if (table.State == TableState.Free)
                {
                    _notifier.SendAsync($"Стол с номером {id}, не занят.");
                    return;
                }

                table?.SetState(TableState.Free);
            }

            _notifier.SendAsync($"Бронь {id} стола снята.");
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