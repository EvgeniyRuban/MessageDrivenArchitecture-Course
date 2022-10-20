namespace Restaurant.Booking;

internal sealed class SqlServerConnectionStringComponents
{
    public string Server { get; set; } = null!;
    public string Database { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string Combine() => $"Server={Server};Database={Database};User Id={UserId};Password={Password}";
}