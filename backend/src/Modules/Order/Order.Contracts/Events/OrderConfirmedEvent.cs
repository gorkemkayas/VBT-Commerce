using MediatR;

namespace Order.Contracts.Events;

/// <summary>
/// Published (via MediatR) by Order.Infrastructure's OrderOutboxProcessor once an order-confirmed
/// outbox row has been read back off order_schema.OutboxMessages — never raised/published directly
/// from a request handler, since the whole point of the Outbox Pattern is to decouple this from the
/// synchronous checkout request (see architecture.md §5). Exactly one of UserId/GuestCustomerId is
/// set, mirroring CustomerOrder's own ownership invariant.
/// </summary>
public record OrderConfirmedEvent(Guid OrderId, Guid? UserId, Guid? GuestCustomerId) : INotification
{
    public const string EventType = "OrderConfirmed";
}
