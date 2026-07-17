using Order.Domain.Enums;

namespace Order.Contracts;

/// <summary>
/// Read-only contract exposed by the Order module to other modules (e.g. the future Review module,
/// which per project-overview.md §4 may only let users who purchased a product write a review).
/// </summary>
public interface IOrderPurchaseVerifier
{
    Task<bool> HasCustomerPurchasedItemAsync(
        Guid userId, Guid sellableItemId, OrderItemType sellableItemType, CancellationToken cancellationToken);
}
