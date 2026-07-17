using Microsoft.EntityFrameworkCore;
using Order.Contracts;
using Order.Domain.Enums;
using Order.Infrastructure.Persistence;

namespace Order.Infrastructure.Integrations;

/// <summary>
/// Implementation of the Order module's outbound contract (see architecture.md §3 —
/// contracts/integrations). Read-only: other modules (e.g. the future Review module) consume this
/// instead of touching <see cref="OrderDbContext"/> directly.
/// </summary>
public class OrderPurchaseVerifierService(OrderDbContext dbContext) : IOrderPurchaseVerifier
{
    public Task<bool> HasCustomerPurchasedItemAsync(
        Guid userId, Guid sellableItemId, OrderItemType sellableItemType, CancellationToken cancellationToken)
    {
        return dbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId && o.Status == OrderStatus.Confirmed)
            .SelectMany(o => o.Items)
            .AnyAsync(i => i.SellableItemId == sellableItemId && i.SellableItemType == sellableItemType, cancellationToken);
    }
}
