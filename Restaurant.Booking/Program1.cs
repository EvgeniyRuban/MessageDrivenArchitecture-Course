using System.Diagnostics;

namespace Restaurant.Booking;

public class Program1
{
    private static readonly List<string> _operations = new()
    {
        "забронировать столик",
        "снять бронь",
    };
    private static readonly List<string> _notifyOperations = new()
    {
        "мы уведомим вас по смс",
        "подождите на линии, мы вас оповестим",
    };

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var restaurant = new Restaurant();
        var stopWatch = new Stopwatch();

        do
        {
            int tableNumber = 0;
            int seatsCount = 0;

            Console.Clear();
            HandleUserInput(_operations, out int operationChoice, true);
            Console.Clear();

            switch (operationChoice)
            {
                case 1:
                    {
                        HandleUserInput(
                            "Укажите количество мест:",
                            out seatsCount,
                            ((int)NumberOfSeats.Single, (int)NumberOfSeats.Twelve),
                            true);

                        break;
                    }
                case 2:
                    {
                        HandleUserInput(
                            "Укажите номер стола:",
                            out tableNumber,
                            (0, int.MaxValue),
                            true);

                        break;
                    }
            }

            Console.Clear();
            HandleUserInput(_notifyOperations, out int operationNotifyChoice, true);

            stopWatch.Start();

            switch (operationChoice)
            {
                case 1:
                    {
                        if (operationNotifyChoice == 1)
                        {
                            restaurant.BookTableAsync(seatsCount);
                        }
                        else
                        {
                            restaurant.BookTable(seatsCount);
                        }

                        break;
                    }
                case 2:
                    {
                        if (operationNotifyChoice == 1)
                        {
                            restaurant.UnbookTableAsync(tableNumber);
                        }
                        else
                        {
                            restaurant.UnbookTable(tableNumber);
                        }

                        break;
                    }
            };

            stopWatch.Stop();

            var ts = stopWatch.Elapsed;
            Console.WriteLine($"Спасибо за ваше обращение! ({ts.Seconds:00}:{ts.Milliseconds:00})");

        } while (Console.ReadKey().Key != ConsoleKey.Escape);
    }

    private static void PrintOperations(List<string> operations)
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

    private static void PrintDataInvalid() => Console.WriteLine("Данные введены некорректно! Попробуйте снова.");

    /// <summary>
    /// Handling user input for <paramref name = "operations"/> by using operation index.
    /// </summary>
    private static void HandleUserInput(
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
            PrintOperations(_operations);
            PrintDataInvalid();
        }
    }

    private static void HandleUserInput(
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
            PrintDataInvalid();
        }
    }
}