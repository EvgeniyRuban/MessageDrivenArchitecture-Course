using MassTransit;
using Restaurant.Messaging;
using System.Collections.Concurrent;

namespace Restaurant.Kitchen;

public sealed class Kitchen
{
    private readonly IBus _bus;
    private readonly IReadOnlyList<Dish> _dishes = new List<Dish>()
    {
        Dish.Pizza, Dish.Lasagna, Dish.Pasta, Dish.Chicken
    };
    private readonly ConcurrentDictionary<Dish, bool> _stoplist = new()
    {
        [Dish.Pizza] = false,
        [Dish.Lasagna] = false,
        [Dish.Chicken] = false,
        [Dish.Pasta] = false,
    };
    private readonly object _lock = new();

    public Kitchen(IBus bus)
    {
        ArgumentNullException.ThrowIfNull(bus, nameof(bus));
        _bus = bus;
    }

    public void CheckKitchenReady(Guid orderId, params Dish[]? dishes)
    {
        bool canCooking = true;

        ResetStoplist();
        TryChangeStoplistRandom(chance: 20);

        if (dishes is not null)
        {
            canCooking = !CheckStoplist(dishes);
        }

        if (!canCooking)
        {
            _bus.Publish<IKitchenReady>(new KitchenReady(orderId, false));
            Console.WriteLine($"Заказ {orderId} не подходит по стоп листу.");
        }

        _bus.Publish<IKitchenReady>(new KitchenReady(orderId, true));
        Console.WriteLine($"Заказ {orderId} принят!");
    }
    /// <summary>
    /// Check if the dish is on the stop list.
    /// </summary>
    /// <param name="dish"></param>
    /// <returns>
    /// Returns true if <paramref name="dish"></paramref> was found in stop list.
    /// </returns>
    private bool CheckStoplist(Dish dish)
    {
        _stoplist.TryGetValue(dish, out bool hasInList);

        return hasInList;
    }
    /// <summary>
    /// Check if the dishes is on the stop list.
    /// </summary>
    /// <param name="dish"></param>
    /// <returns>
    /// Returns true if <paramref name="dish"></paramref> was found in stop list.
    /// </returns>
    private bool CheckStoplist(params Dish[] dishes)
    {
        bool hasInList = default;
        for (int i = 0; i < dishes.Length; i++)
        {
            _stoplist.TryGetValue(dishes[i], out hasInList);

            if (hasInList)
            {
                break;
            }
        }

        return hasInList;
    }
    /// <summary>
    /// Randomly changing one of stop list postions by true setting, according to <paramref name="chance"/>.
    /// </summary>
    /// <param name="chance">Chance to update one of _stoplist values.</param>
    private void TryChangeStoplistRandom(int chance)
    {
        var rnd = new Random();

        if (rnd.Next(100) < chance)
        {
            var dishToAdd = (Dish)rnd.Next(_dishes.Count);
            _stoplist.TryUpdate(dishToAdd, true, false);
        }
    }
    private void ResetStoplist()
    {
        lock (_lock)
        {
            foreach (var pair in _stoplist)
            {
                if (pair.Value)
                {
                    _stoplist[pair.Key] = false;
                }
            }
        }
    }
}