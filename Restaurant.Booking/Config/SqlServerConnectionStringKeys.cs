namespace Restaurant.Booking;

internal struct SqlServerConnectionComponentsKeys
{
    public const string Server = $"{_section}:Server";
    public const string Database = $"{_section}:Database";
    public const string UserId = $"{_section}:UserId";
    public const string Password = $"{_section}:Password";

    private const string _section = nameof(SqlServerConnectionStringComponents);
}