namespace Restaurant.Booking;

internal struct RabbitMqHostSettingsKeys
{
    public const string Host = $"{_section}:Host";
    public const string VirtualHost = $"{_section}:VirtualHost";
    public const string User = $"{_section}:User";
    public const string Password = $"{_section}:Password";
    public const string Port = $"{_section}:Port";

    private const string _section = "RabbitMqHostSettings";
}
