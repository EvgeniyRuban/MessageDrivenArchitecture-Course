namespace Restaurant.Booking;

internal sealed class RabbitMqHostConfig
{
    public string Host { get; } = null!;
    public string VirtualHost { get; } = null!;
    public string User { get; } = null!;
    public string Password { get; } = null!;
    public ushort Port { get; }
}