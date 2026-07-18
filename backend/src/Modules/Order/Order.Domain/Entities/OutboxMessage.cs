namespace Order.Domain.Entities;

/// <summary>
/// Written in the same transaction/SaveChanges call as the order-state change it announces (see
/// architecture.md §5 — Outbox Pattern), so the two either both commit or both roll back together.
/// OrderOutboxProcessor polls this table independently of the HTTP request — long after that
/// transaction has already committed — and republishes each unprocessed row as its corresponding
/// MediatR notification.
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string EventType { get; private set; } = null!;
    public string Payload { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private OutboxMessage()
    {
    }

    public static OutboxMessage Create(string eventType, string payload)
        => new() { Id = Guid.NewGuid(), EventType = eventType, Payload = payload, CreatedAt = DateTime.UtcNow };

    public void MarkProcessed() => ProcessedAt = DateTime.UtcNow;
}
