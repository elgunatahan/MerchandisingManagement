using Domain.Entities;
using Newtonsoft.Json;

namespace Domain.Common.Factories
{
    public class OutboxMessageFactory : IOutboxMessageFactory
    {
        public OutboxMessage From<T>(T @event, DateTime now) where T : class, new()
        {
            string data = JsonConvert.SerializeObject(@event);

            string type = @event.GetType().FullName;

            OutboxMessage outboxMessage = new OutboxMessage(now, type, data);

            return outboxMessage;
        }
    }
}
