namespace RestaurantApp;

public sealed class Table
{
    public Table(int id, int seatsCount)
    {
        Id = id;
        State = TableState.Free;
        SeatsCount = seatsCount;
    }

    public int Id { get; }
    public TableState State { get; private set; }
    public int SeatsCount { get; }

    public void SetState(TableState state) => State = state;

}