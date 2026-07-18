using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Order.Contracts.Events;
using Order.Infrastructure.Persistence;

namespace Order.Infrastructure.Outbox;

/// <summary>
/// Polls order_schema.OutboxMessages independently of any HTTP request (see architecture.md §5 —
/// Outbox Pattern) and republishes each unprocessed row as its corresponding MediatR notification.
/// This is what keeps checkout synchronous-but-fast: OrderOperations.ConfirmAsync only ever writes a
/// row to this table (in the same transaction as the order-confirm), it never calls Publish itself
/// or waits on any downstream side effect (e.g. sending an email).
/// </summary>
public class OrderOutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OrderOutboxProcessor> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 20;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(PollInterval);

        do
        {
            try
            {
                await ProcessPendingMessagesAsync(stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Unexpected failure while processing the order outbox.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        var pendingMessages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in pendingMessages)
        {
            try
            {
                await DispatchAsync(message.EventType, message.Payload, publisher, cancellationToken);
                message.MarkProcessed();
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Failed to dispatch outbox message '{MessageId}' of type '{EventType}'; it will be retried on the next poll.",
                    message.Id, message.EventType);
            }
        }
    }

    private static Task DispatchAsync(string eventType, string payload, IPublisher publisher, CancellationToken cancellationToken)
    {
        return eventType switch
        {
            OrderConfirmedEvent.EventType => publisher.Publish(
                JsonSerializer.Deserialize<OrderConfirmedEvent>(payload)
                    ?? throw new InvalidOperationException($"Outbox payload for '{eventType}' deserialized to null."),
                cancellationToken),
            _ => throw new InvalidOperationException($"Unknown outbox event type '{eventType}'.")
        };
    }
}
