namespace Restaurant.Kitchen;

internal sealed class RabbitMQHostConfig
{
    public string HostName { get; } = null!;
    public string VirtualHost { get; } = null!;
    public string UserName { get; } = null!;
    public string Password { get; } = null!;
    public ushort Port { get; }
}