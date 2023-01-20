using Domain.Enums;

namespace Domain.Entities
{
    public class OutboxMessage
    {
        public long Id { get; set; }

        public string Data { get; }
        
        public DateTime OccurredOn { get; }

        public DateTime? ProcessedDate { get; set; }
        
        public string QueueName { get; set; }

        public string RoutingKey { get; set; }

        public OutboxMessageStatus Status { get; set; }
        
        public string Type { get; }

        public OutboxMessage(DateTime occurredOn, string type, string data)
        {
            OccurredOn = occurredOn;
            Type = type;
            Data = data;
            Status = OutboxMessageStatus.New;
        }

        public OutboxMessage()
        {

        }
    }
}
