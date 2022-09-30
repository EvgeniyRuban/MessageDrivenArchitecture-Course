using MassTransit;
using Microsoft.Extensions.Hosting;
using Restaurant.Messaging;

namespace Restaurant.Booking
{
    public sealed class Worker : BackgroundService
    {
        private readonly IBus _bus;
        private readonly Restaurant _restaurant;

        public Worker(IBus bus, Restaurant restaurant)
        {
            ArgumentNullException.ThrowIfNull(bus, nameof(bus));
            ArgumentNullException.ThrowIfNull(restaurant, nameof(restaurant));

            _bus = bus;
            _restaurant = restaurant;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10_000, stoppingToken);
                Console.WriteLine("Привет, желаете забронировать столик?");
                await _restaurant.BookTableAsync(1);
                await _bus.Publish(new TableBooked(Guid.NewGuid(), Guid.NewGuid(), false), stoppingToken);
            }
        }
    }
}