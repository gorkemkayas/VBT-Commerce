using Cart.Domain.Enums;

namespace Cart.Application.Integrations;

/// <summary>
/// Cart's own view of what it needs from the Inventory module. Only used for a soft, non-reserving
/// UX check when adding/updating a line — the real stock reservation happens later, when Order is
/// created (architecture.md §6.1). Implemented in Cart.Infrastructure against Inventory's
/// IInventoryService.
/// </summary>
public interface IInventoryIntegrationService
{
    Task<int> GetAvailableQuantityAsync(Guid sellableItemId, CartItemType sellableItemType, CancellationToken cancellationToken);
}
