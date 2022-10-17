using System.Collections.Concurrent;
using Restaurant.Messaging;

namespace Restaurant.Kitchen;

internal sealed class Kitchen
{
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

    public bool CheckKitchenReady(params Dish[]? dishes) => dishes is null ? true : CheckStoplist(dishes);
    /// <summary>
    /// Check if the <paramref name="dish"></paramref> is on the stop list.
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
    /// Check if the <paramref name="dishes"></paramref> is on the stop list.
    /// </summary>
    /// <param name="dishes"></param>
    /// <returns>
    /// Returns true if <paramref name="dishes"></paramref> were found in stop list.
    /// </returns>
    private bool CheckStoplist(params Dish[] dishes)
    {
        bool hasInList = false;

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