using Inventory.Domain.Enums;

namespace ECommerce.API.Controllers.Inventory;

public record ReserveStockLineItemRequest(Guid SellableItemId, InventoryItemType SellableItemType, int Quantity);

public record ReserveStockRequest(Guid ReferenceId, IReadOnlyCollection<ReserveStockLineItemRequest> Items);
