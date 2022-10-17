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

        Event(() => GuestArrived,
            x =>
                x.CorrelateById(context => context.Message.OrderId));

        Event(() => TableBookedFault,
            x =>
                x.CorrelateById(m => m.Message.Message.OrderId));

        Event(() => KitchenReadyFault,
            x =>
                x.CorrelateById(m => m.Message.Message.OrderId));

        Event(() => BookingRequestedFault,
            x =>
                x.CorrelateById(m => m.Message.Message.OrderId));

        Event(() => GuestArrivedFault,
            x =>
                x.CorrelateById(m => m.Message.Message.OrderId));

        CompositeEvent(() => BookingApproved,
            x => x.ReadyEventStatus, KitchenReady, TableBooked);

        Schedule(() => BookingExpired,
            x => x.ExpirationId, x =>
            {
                x.Delay = TimeSpan.FromSeconds(1);
                x.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });

        Schedule(() => GuestArrivalExpired,
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
                    context.Instance.GuestArrivalVia = context.Data.ArriveVia;
                    Console.WriteLine($"[Order: {context.Data.OrderId}] - ожидается подтвеждения брони.");
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
                                        "стол успешно забронирован"))
                .Publish(context =>
                    (IBookingApproved)new BookingApproved(context.Instance.OrderId, context.Instance.ClientId))
                .Schedule(GuestArrivalExpired,
                    context => new GuestArrivalExpired(context.Instance),
                    context => context.Instance.GuestArrivalVia)
                .TransitionTo(AwaitingGuestArrived),

            When(TableBooked)
                .Then(context => context.Instance.TableId = context.Data.TableId),

            When(TableBookedFault)
                .Then(context => Console.WriteLine($"[Order: {context.Instance.OrderId}] - произошла ошибка."))
                .Publish(context => (IBookingCancelled) new BookingCancelled(context.Instance.OrderId, context.Instance.TableId))
                .Publish(context => (INotify)new Notify(context.Instance.OrderId,
                                                        context.Instance.ClientId,
                                                        "приносим извинения, стол забронировать не получилось"))
                .Finalize(),

            When(KitchenReadyFault)
                .Then(context => Console.WriteLine($"[Order: {context.Instance.OrderId}] - произошла ошибка."))
                .Publish(context => (IBookingCancelled)new BookingCancelled(context.Instance.OrderId, context.Instance.TableId))
                .Publish(context => (INotify)new Notify(context.Instance.OrderId,
                                                        context.Instance.ClientId,
                                                        "приносим извинения, стол забронировать не получилось"))
            .Finalize(),

            When(BookingRequestedFault)
                .Then(context => Console.WriteLine($"[Order: {context.Instance.OrderId}] - произошла ошибка."))
                .Publish(context => (IBookingCancelled) new BookingCancelled(context.Instance.OrderId, context.Instance.TableId))
                .Publish(context => (INotify)new Notify(context.Instance.OrderId,
                                                        context.Instance.ClientId,
                                                        "приносим извинения, стол забронировать не получилось"))
                .Finalize(),

            When(BookingExpired.Received)
                .Then(context => Console.WriteLine($"[Order: {context.Instance.OrderId}] - отмена заказа."))
                .Publish(context => new BookingCancelled(context.Instance.OrderId, context.Instance.TableId))
                .Finalize()
        );

        During(AwaitingGuestArrived,
            When(GuestArrived)
                .Unschedule(GuestArrivalExpired)
                .Then(context => Console.WriteLine($"[Order: {context.Instance.OrderId}] - гость прибыл."))
                .Finalize(),

            When(GuestArrivalExpired.Received)
                .Then(context => Console.WriteLine($"[Order: {context.Instance.OrderId}] - гость не пришел."))
                .Publish(context => new BookingCancelled(context.Instance.OrderId, context.Instance.TableId))
                .Publish(context => new Notify(context.Instance.OrderId,
                                               context.Instance.ClientId,
                                               "ваша бронь снята по истечению времени"))
                .Finalize(),

            When(GuestArrivedFault)
                .Then(context => Console.WriteLine($"[Order: {context.Instance.OrderId}] - произошла ошибка."))
                .Publish(context => (IBookingCancelled) new BookingCancelled(context.Instance.OrderId, context.Instance.TableId))
                .Publish(context => (INotify)new Notify(context.Instance.OrderId,
                                                        context.Instance.ClientId,
                                                        "приносим извинения, стол забронировать не получилось"))
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

    public Event BookingApproved { get; private set; }

    public Event<Fault<ITableBooked>> TableBookedFault { get; private set; }
    public Event<Fault<IKitchenReady>> KitchenReadyFault { get; private set; }
    public Event<Fault<IBookingRequested>> BookingRequestedFault { get; private set; }
    public Event<Fault<IGuestArrived>> GuestArrivedFault { get; private set; }

    public Schedule<BookingState, IBookingExpired> BookingExpired { get; private set; }
    public Schedule<BookingState, IGuestArrivalExpired> GuestArrivalExpired { get; private set; }
}