using Microsoft.Data.SqlClient;
using OutboxJob.Services;

namespace OutboxJobs.Jobs
{
    public class OutboxMessagePublisherHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxMessagePublisherHostedService> _logger;
         private static readonly Random Jitterer = new Random();

        public OutboxMessagePublisherHostedService(IServiceProvider serviceProvider,
                                                   ILogger<OutboxMessagePublisherHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PublishMessages(stoppingToken);
             
                int delayTime = Jitterer.Next(200, 400);
                
                await Task.Delay(delayTime, stoppingToken);
            }
        }
 
        private async Task PublishMessages(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IOutboxMessagePublisherService outboxServicePublisher = scope.ServiceProvider.GetRequiredService<IOutboxMessagePublisherService>();

                try
                {
                    await outboxServicePublisher.Publish(cancellationToken);
                }
                catch (SqlException sqlException) when (sqlException.Message.Contains("deadlocked on lock resources with another process"))
                {
                    _logger.LogWarning(sqlException, sqlException.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}
