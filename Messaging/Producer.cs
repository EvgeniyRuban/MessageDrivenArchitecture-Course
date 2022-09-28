using System.Text;
using RabbitMQ.Client;

namespace Messaging;

public sealed class Producer
{
    private readonly ProducerConfig _config;

    public Producer(ProducerConfig config)
    {
        ArgumentNullException.ThrowIfNull(config, nameof(config));
        _config = new()
        {
            UserName = config.UserName ?? throw new ArgumentNullException(config.UserName, nameof(config.UserName)),
            Password = config.Password ?? throw new ArgumentNullException(config.Password, nameof(config.Password)),
            HostName = config.HostName ?? throw new ArgumentNullException(config.HostName, nameof(config.HostName)),
            VirtualHost = config.VirtualHost ?? throw new ArgumentNullException(config.VirtualHost, nameof(config.VirtualHost)),
            QueueName = config.QueueName ?? throw new ArgumentNullException(config.QueueName, nameof(config.QueueName)),
            RoutingKey = config.RoutingKey ?? throw new ArgumentNullException(config.RoutingKey, nameof(config.RoutingKey)),
            ExchangeName = config.ExchangeName ?? throw new ArgumentNullException(config.ExchangeName, nameof(config.ExchangeName)),
            Port = config.Port,
        };
    }

    public void Send(string message)
    {
        var factory = new ConnectionFactory()
        {
            UserName = _config.UserName,
            HostName = _config.HostName,
            VirtualHost = _config.VirtualHost,
            Port = _config.Port,
            Password = _config.Password,
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: _config.ExchangeName,
                                type: ExchangeType.Direct,
                                durable: false,
                                autoDelete: false,
                                arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: _config.ExchangeName,
                             routingKey: _config.RoutingKey,
                             basicProperties: null,
                             body: body);
    }
}