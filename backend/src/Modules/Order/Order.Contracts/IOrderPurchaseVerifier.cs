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

    /// <summary>
    /// Distinct variant IDs the customer has a Confirmed order for. Lets callers (Review) resolve
    /// "purchased variant X" up to "purchased variant X's parent product" without Order needing to
    /// know about Catalog's product/variant hierarchy itself.
    /// </summary>
    Task<IReadOnlyCollection<Guid>> GetPurchasedVariantIdsAsync(Guid userId, CancellationToken cancellationToken);
}
