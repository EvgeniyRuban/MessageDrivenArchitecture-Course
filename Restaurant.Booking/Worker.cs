using Microsoft.Extensions.Hosting;
using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking;

public sealed class Worker : BackgroundService
{
    private readonly IBus _bus;
    private readonly Restaurant _restaurant;
    private readonly List<string> _operations = new List<string>()
    {
        "забронировать столик",
        "снять бронь",
    };

    public Worker(IBus bus, Restaurant restaurant)
    {
        ArgumentNullException.ThrowIfNull(bus, nameof(bus));
        ArgumentNullException.ThrowIfNull(restaurant, nameof(restaurant));

        _bus = bus;
        _restaurant = restaurant;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            int numberOfSeats = 0;
            int tableNumber = 0;

            Console.WriteLine("Здравствуйте!");

            HandleUserInput(_operations, out int operationChoice, true);

            Console.Clear();

            Task<int?> bookingResultTask = null!;
            Task<bool?> unbookingResultTask = null!;

            switch (operationChoice)
            {
                case 1:
                    {
                        HandleUserInput(
                            "Укажите количество мест:",
                            out numberOfSeats,
                            ((int)NumberOfSeats.Single, (int)NumberOfSeats.Twelve),
                            true);

                        bookingResultTask = _restaurant.BookTableAsync(numberOfSeats);

                        break;
                    }
                case 2:
                    {
                        HandleUserInput("Укажите номер стола:", out tableNumber, (0, int.MaxValue), true);

                        unbookingResultTask = _restaurant.UnbookTableAsync(tableNumber);

                        break;
                    }
            }

            Console.WriteLine("Благодарим за ваше обращение, ответ будет отправлен по смс.");

            await Task.WhenAll(
                bookingResultTask ?? Task.CompletedTask,
                unbookingResultTask ?? Task.CompletedTask);

            switch (operationChoice)
            {
                case 1:
                    {
                        var bookingResult = bookingResultTask.Result;

                        if (bookingResult is null)
                        {
                            Console.WriteLine("К сожалению, в данный момент подходящих для вас столиков нет.");
                        }
                        else
                        {
                            await _bus.Publish<TableBooked>(new(Guid.NewGuid(), Guid.NewGuid(), false), stoppingToken);
                            Console.WriteLine($"Для вас забронирован стол с номером {bookingResult}");
                        }

                        break;
                    }
                case 2:
                    {
                        var unbookingResult = unbookingResultTask.Result;

                        if (unbookingResult is null)
                        {
                            Console.WriteLine("Стола с указаным вами номером не существует.");
                        }
                        else if (unbookingResult == false)
                        {
                            Console.WriteLine("Стола с указаным вами номером не забронирован.");
                        }
                        else
                        {
                            Console.WriteLine($"Бронь на стол номер {tableNumber}, была аннулирована.");
                        }

                        break;
                    }
            }

            Console.ReadKey();
            Console.Clear();

        } while (!stoppingToken.IsCancellationRequested);
    }

    private void PrintOperations(List<string> operations)
    {
        for (int i = 0; i < operations.Count; i++)
        {
            if (i != operations.Count - 1)
            {
                Console.WriteLine($"[{i + 1}] - {operations[i]};");
            }
            else
            {
                Console.WriteLine($"[{i + 1}] - {operations[i]}.");
            }
        }
    }

    /// <summary>
    /// Handling user <paramref name = "input"/> for valid integer range.
    /// </summary>
    /// <param name="retryMessage">Printing every cycle.</param>
    /// <param name="input">User input.</param>
    /// <param name="validRange">Valid user input range.</param>
    /// <param name="untilCorrectInput">Should method continue handling user input, after first incorrect.</param>
    private void HandleUserInput(
    string retryMessage,
    out int input,
    (int min, int max) validRange,
    bool untilCorrectInput = default)
    {
        Console.WriteLine(retryMessage);
        while (!int.TryParse(Console.ReadLine(), out input)
               || input > validRange.max
               || input < validRange.min
               && untilCorrectInput)
        {
            Console.Clear();
            Console.WriteLine(retryMessage);
            Console.WriteLine("Введенные данные некорректны!");
        }
    }

    /// <summary>
    /// Handling user <paramref name = "input"/> for <paramref name = "operations"/> by using operation index.
    /// </summary>
    /// <param name="operations">Printing every cycle.</param>
    /// <param name="input">User input.</param>
    /// <param name="untilCorrectInput">Should method continue handling user input, after first incorrect.</param>
    private void HandleUserInput(
        List<string> operations,
        out int input,
        bool untilCorrectInput = default)
    {
        PrintOperations(operations);
        while (!int.TryParse(Console.ReadLine(), out input)
               || input - 1 > operations.Count - 1
               || input - 1 < 0
               && untilCorrectInput)
        {
            Console.Clear();
            PrintOperations(operations);
            Console.WriteLine();
        }
    }
}