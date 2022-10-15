﻿using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Notification;

public sealed class NotifyConsumer : IConsumer<INotify>
{
    private readonly Notifier _notifier;

    public NotifyConsumer(Notifier notifier)
    {
        ArgumentNullException.ThrowIfNull(notifier, nameof(notifier));

        _notifier = notifier;
    }

    public Task Consume(ConsumeContext<INotify> context)
    {
        _notifier.Notify(context.Message.OrderId);

        return context.ConsumeCompleted;

    }
}