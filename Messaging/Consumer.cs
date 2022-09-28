using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging;

public sealed class Consumer : IDisposable
{
    private readonly ConsumerConfig _config;

    private readonly IConnection _connection;
    private readonly IModel _channel;

    public Consumer(ConsumerConfig config)
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

        var factory = new ConnectionFactory()
        {
            UserName = _config.UserName,
            HostName = _config.HostName,
            VirtualHost = _config.VirtualHost,
            Port = _config.Port,
            Password = _config.Password,
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Receive(EventHandler<BasicDeliverEventArgs> receiveCallBack)
    {
        _channel.QueueDeclare(queue: _config.QueueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        _channel.QueueBind(queue: _config.QueueName,
                           exchange: _config.ExchangeName,
                           routingKey: _config.RoutingKey);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += receiveCallBack;

        _channel.BasicConsume(queue: _config.QueueName,
                              autoAck: true,
                              consumer: consumer);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _channel?.Dispose();
    }
}