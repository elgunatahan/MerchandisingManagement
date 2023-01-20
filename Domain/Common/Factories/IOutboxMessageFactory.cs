using Domain.Entities;

namespace Domain.Common.Factories
{
    public interface IOutboxMessageFactory
    {
        OutboxMessage From<T>(T @event, DateTime now) where T : class, new();
    }
}
