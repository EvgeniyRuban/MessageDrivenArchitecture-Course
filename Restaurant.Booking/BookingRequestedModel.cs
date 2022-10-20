using Restaurant.Messaging;

namespace Restaurant.Booking
{
    public sealed class BookingRequestedModel : IBookingRequested
    {
        private readonly List<string> _messagesIds = new();

        public BookingRequestedModel(Guid orderId,
                                     Guid clientId,
                                     TimeSpan arriveVia,
                                     DateTime creationDate,
                                     string messageId,  
                                     Dish[]? preOrder = null)
        {
            OrderId = orderId;
            ClientId = clientId;
            ArriveVia = arriveVia;
            CreationDate = creationDate;
            _messagesIds.Add(messageId);
            PreOrder = preOrder;
        }

        public Guid OrderId { get; private set; }
        public Guid ClientId { get; private set; }
        public TimeSpan ArriveVia { get; private set; }
        public Dish[]? PreOrder { get; private set; }
        public DateTime CreationDate { get; private set; }

        public BookingRequestedModel Update(BookingRequestedModel model, string messageId)
        {
            _messagesIds.Add(messageId);

            OrderId = model.OrderId;
            ClientId = model.ClientId;
            ArriveVia = model.ArriveVia;
            CreationDate = model.CreationDate;
            PreOrder = model.PreOrder;

            return this;
        }
        public bool CheckMessageId(string messageId) => _messagesIds.Contains(messageId);
    }
}
