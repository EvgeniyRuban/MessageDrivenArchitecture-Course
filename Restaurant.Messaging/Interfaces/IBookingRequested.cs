﻿namespace Restaurant.Messaging;

public interface IBookingRequested
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish[]? PreOrder { get; }
    public DateTime CreationDate { get; }
}