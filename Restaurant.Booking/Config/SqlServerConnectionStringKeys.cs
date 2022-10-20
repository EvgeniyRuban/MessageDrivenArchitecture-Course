namespace Restaurant.Booking;

internal struct SqlServerConnectionComponentsKeys
{
    public const string Server = $"{_section}:{_subSection}:Server";
    public const string Database = $"{_section}:{_subSection}:Database";
    public const string UserId = $"{_section}:{_subSection}:UserId";
    public const string Password = $"{_section}:{_subSection}:Password";

    private const string _section = nameof(SqlServerSettings);
    private const string _subSection = nameof(SqlServerConnectionStringComponents);
}