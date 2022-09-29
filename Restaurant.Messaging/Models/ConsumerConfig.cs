namespace Messaging;

public sealed class ConsumerConfig
{
    public string HostName { get; set; } = null!;
    public string VirtualHost { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int Port { get; set; }
    public string QueueName { get; set; } = null!;
    public string RoutingKey { get; set; } = null!;
    public string Exchange { get; set; } = null!;
}