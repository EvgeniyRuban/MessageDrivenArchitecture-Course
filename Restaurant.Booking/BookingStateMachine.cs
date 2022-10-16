using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking;

public sealed class BookingStateMachine : MassTransitStateMachine<BookingState>
{
    public BookingStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingRequested,
            x =>
                x.CorrelateById(context => context.Message.OrderId)
                    .SelectId(context => context.Message.OrderId));

        Event(() => TableBooked,
            x =>
                x.CorrelateById(context => context.Message.OrderId));

        Event(() => KitchenReady,
            x =>
                x.CorrelateById(context => context.Message.OrderId));

        Event(() => BookingRequestedFault,
            x =>
                x.CorrelateById(m => m.Message.Message.OrderId));

        Event(() => GuestArrived,
            x =>
                x.CorrelateById(context => context.Message.OrderId));

        CompositeEvent(() => BookingApproved,
            x => x.ReadyEventStatus, KitchenReady, TableBooked);

        Schedule(() => BookingExpired,
            x => x.ExpirationId, x =>
            {
                x.Delay = TimeSpan.FromSeconds(1);
                x.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });

        Initially(
            When(BookingRequested)
                .Then(context =>
                {
                    context.Instance.CorrelationId = context.Data.OrderId;
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.ClientId = context.Data.ClientId;
                    Console.WriteLine("[State: Awaiting booking approved " + context.Data.CreationDate);
                })
                .Schedule(BookingExpired,
                    context => new BookingExpired(context.Instance),
                    context => TimeSpan.FromSeconds(10))
                .TransitionTo(AwaitingBookingApproved)
        );

        During(AwaitingBookingApproved,
            When(BookingApproved)
                .Unschedule(BookingExpired)
                .Publish(context =>
                    (INotify)new Notify(context.Instance.OrderId,
                                        context.Instance.ClientId,
                                        "Стол успешно забронирован"))
                .Publish(context =>
                    (IBookingApproved)new BookingApproved(context.Instance.OrderId, context.Instance.ClientId))
            .TransitionTo(AwaitingGuestArrived),

            When(BookingRequestedFault)
                .Then(context => Console.WriteLine($"Ошибочка вышла!"))
                .Publish(context => (INotify)new Notify(context.Instance.OrderId,
                    context.Instance.ClientId,
                    "Приносим извинения, стол забронировать не получилось"))
                .Finalize(),

            When(BookingExpired.Received)
                .Then(context => Console.WriteLine($"Отмена заказа {context.Instance.OrderId}."))
                .Finalize()
        );

        During(AwaitingGuestArrived,
            When(GuestArrived)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    public State AwaitingBookingApproved { get; private set; }
    public State AwaitingGuestArrived { get; private set; }

    public Event<ITableBooked> TableBooked { get; private set; }
    public Event<IKitchenReady> KitchenReady { get; private set; }
    public Event<IBookingRequested> BookingRequested { get; private set; }
    public Event<IGuestArrived> GuestArrived { get; private set; }


    public Event<Fault<IBookingRequested>> BookingRequestedFault { get; private set; }

    public Schedule<BookingState, IBookingExpired> BookingExpired { get; private set; }
    public Event BookingApproved { get; private set; }
}