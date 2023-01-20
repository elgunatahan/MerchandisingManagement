namespace OutboxJob.Services
{
    public interface IOutboxMessagePublisherService
    {
        Task Publish(CancellationToken cancellationToken);
    }
}
