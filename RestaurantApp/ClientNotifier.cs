namespace RestaurantApp;

public sealed class ClientNotifier
{
    private readonly TimeSpan _messageSendingDelay = TimeSpan.FromSeconds(5);

    public async Task SendAsync(string message)
    {
        Task.Run(async () =>
        {
            Task.Delay(_messageSendingDelay).Wait();
            Console.WriteLine(message);
        });
    }
}