using MassTransit;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OutboxJob.Services;
using OutboxJobs.Jobs;
using RabbitMQ.Client;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddMemoryCache();

builder.Services.AddHttpContextAccessor();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        //cfg.Host(Environment.GetEnvironmentVariable("RABBITMQ_HOST"), "", h => { });
        cfg.Host(new Uri($"rabbitmq://{Environment.GetEnvironmentVariable("RABBITMQ_HOST")}/guest"), c =>
     {
         c.Username("guest");
         c.Password("guest");
     });

#if DEBUG
        cfg.UseConcurrencyLimit(1);
#endif
        cfg.PrefetchCount = 16;
        cfg.UseConcurrencyLimit(16);

        cfg.UseRetry(configurator =>
        {
            configurator.Incremental(20, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(1000));
            configurator.Ignore<ApplicationException>();
        });
    });
});


string redisConnectionString = Environment.GetEnvironmentVariable("REDIS_HOST");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});

ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
RedisDistributedSynchronizationProvider lockProvider = new RedisDistributedSynchronizationProvider(connectionMultiplexer.GetDatabase(), c =>
{
    c.Expiry(TimeSpan.FromSeconds(10));
    c.BusyWaitSleepTime(TimeSpan.FromMilliseconds(3), TimeSpan.FromMilliseconds(10));
});

builder.Services.AddSingleton(_ => lockProvider);

builder.Services.AddScoped<IOutboxMessagePublisherService, OutboxMessagePublisherService>(); 

builder.Services.AddHostedService<OutboxMessagePublisherHostedService>();


var connection = new ConnectionFactory()
{
    HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
    VirtualHost = "/",
    UserName = "guest",
    Password = "guest"
};

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "Liveness" })
    .AddRabbitMQ(connectionFactory: (prov) => connection.CreateConnection(),
        "RabbitMq", tags: new[] { "Readiness" });

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/live", new HealthCheckOptions { Predicate = p => p.Tags.Contains("Liveness") });
    endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = p => p.Tags.Contains("Readiness") });
});

app.Run();