using Dapper;
using Domain.Entities;
using Domain.Events;
using MassTransit;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using System.Reflection;
using System.Text.Json;

namespace OutboxJob.Services
{
    public class OutboxMessagePublisherService : IOutboxMessagePublisherService
    {
        private readonly IBusControl _busControl;
        private readonly ILogger<OutboxMessagePublisherService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly RedisDistributedSynchronizationProvider _distributedSynchronizationProvider;

        public OutboxMessagePublisherService(IBusControl busControl,
            ILogger<OutboxMessagePublisherService> logger,
            IConfiguration configuration,
            ISendEndpointProvider sendEndpointProvider,
            RedisDistributedSynchronizationProvider distributedSynchronizationProvider)
        {
            _busControl = busControl;
            _logger = logger;
            _configuration = configuration;
            _sendEndpointProvider = sendEndpointProvider;
            _distributedSynchronizationProvider = distributedSynchronizationProvider;
        }

        public async Task Publish(CancellationToken cancellationToken)
        {
            IEnumerable<OutboxMessage> outboxMessages = await GetOutboxMessages(cancellationToken);

            Parallel.ForEach(outboxMessages, message =>
            {
                FallbackPolicy fallbackPolicy = Policy
                                                    .Handle<Exception>()
                                                    .Fallback(token =>
                                                    {
                                                        _logger.LogError($"Outbox message publish failed. MessageId: {message.Id} MessageType: {message.Type}");
                                                    });

                RetryPolicy retryPolicy = Policy
                                              .Handle<Exception>()
                                              .WaitAndRetry(5,
                                                                 retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt))
                                                               , (exception, timeSpan, retryCount, context) =>
                                                               {
                                                                   _logger.LogWarning(exception, $"Outbox message publish failed. RetryCount: {retryCount} MessageId: {message.Id} MessageType: {message.Type}");
                                                               });

                fallbackPolicy
                   .Wrap(retryPolicy)
                   .Execute(() => ProcessMessage(message).Wait());
            });
        }

        private async Task<IEnumerable<OutboxMessage>> GetOutboxMessages(CancellationToken stoppingToken)
        {
            using (await _distributedSynchronizationProvider.AcquireLockAsync("OutboxJobs", cancellationToken: stoppingToken))
            {
                using (SqlConnection conn = new SqlConnection(_configuration["DB_URL"]))
                {
                    DateTime utcNow = DateTime.UtcNow;
                    DateTime before = DateTime.UtcNow.AddMinutes(-1);

                    return await conn.QueryAsync<OutboxMessage>($@"UPDATE TOP(42) OutboxMessages 
                                                                    SET Status=2,
                                                                        ProcessedDate=@ProcessedDate
                                                                    OUTPUT Inserted.Id,
                                                                           Inserted.[Type],
                                                                           Inserted.[Data],
                                                                           Inserted.[Status],
                                                                           Inserted.ProcessedDate,
                                                                           Inserted.QueueName,
                                                                           Inserted.RoutingKey
                                                                    WHERE 
                                                                           (Status=1 OR (Status=2 AND ProcessedDate<=@BeforeProcessedDate))
                                                                   ",
                        new { ProcessedDate = utcNow, BeforeProcessedDate = before });
                }
            }
        }

        private async Task ProcessMessage(OutboxMessage outboxMessage)
        {
            await PublishOrSendMessage(outboxMessage);

            await UpdateMessageStatusAsPublished(outboxMessage);
        }

        private async Task PublishOrSendMessage(OutboxMessage outboxMessage)
        {
            Assembly asm = typeof(ProductCreated).Assembly;

            Type type = asm.GetType(outboxMessage.Type);

            object domainEvent = JsonSerializer.Deserialize(outboxMessage.Data, type);

            if (string.IsNullOrWhiteSpace(outboxMessage.QueueName))
            {
                await Publish(domainEvent, outboxMessage.RoutingKey);
            }
            else
            {
                await Send(domainEvent, outboxMessage.QueueName);
            }

            _logger.LogInformation("Outbox message was published. OutboxMessageId: {Id}", outboxMessage.Id);
        }

        private async Task Publish(Object domainEvent, string routingKey)
        {
            try
            {
                await _busControl.Publish(domainEvent, p =>
                {
                    if (!string.IsNullOrWhiteSpace(routingKey))
                    {
                        p.SetRoutingKey(routingKey);
                    }
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task Send(object domainEvent, string queueName)
        {
            ISendEndpoint endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));

            await endpoint.Send(domainEvent);
        }

        private async Task UpdateMessageStatusAsPublished(OutboxMessage outboxMessage)
        {
            DateTime utcNow = DateTime.UtcNow;
            using (SqlConnection conn = new SqlConnection(_configuration["DB_URL"]))
            {
                await conn.ExecuteAsync(@"UPDATE OutboxMessages
                                              SET Status=3,
                                                  ProcessedDate=@ProcessedDate
                                              WHERE
                                                  Id=@Id", new
                {
                    Id = outboxMessage.Id,
                    ProcessedDate = utcNow
                });
            }
        }
    }
}
