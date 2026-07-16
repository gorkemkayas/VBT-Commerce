using Inventory.Domain.Enums;

namespace Inventory.Contracts;

public record ReserveStockItemRequest(Guid SellableItemId, InventoryItemType SellableItemType, int Quantity);
