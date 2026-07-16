using Cart.Application.Integrations;
using Cart.Domain.Enums;
using Inventory.Contracts;
using InventoryItemType = Inventory.Domain.Enums.InventoryItemType;

namespace Cart.Infrastructure.Integrations;

/// <summary>
/// Adapts Cart's own <see cref="IInventoryIntegrationService"/> to Inventory's actual contract
/// (<see cref="IInventoryService"/>), so Cart.Application never references Inventory directly.
/// </summary>
public class InventoryIntegrationService(IInventoryService inventoryService) : IInventoryIntegrationService
{
    public Task<int> GetAvailableQuantityAsync(Guid sellableItemId, CartItemType sellableItemType, CancellationToken cancellationToken)
    {
        var inventoryItemType = sellableItemType switch
        {
            CartItemType.Product => InventoryItemType.Product,
            CartItemType.Variant => InventoryItemType.Variant,
            _ => throw new ArgumentOutOfRangeException(nameof(sellableItemType))
        };

        return inventoryService.GetAvailableQuantityAsync(sellableItemId, inventoryItemType, cancellationToken);
    }
}
