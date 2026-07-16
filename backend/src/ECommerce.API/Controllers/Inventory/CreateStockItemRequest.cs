using Inventory.Domain.Enums;

namespace ECommerce.API.Controllers.Inventory;

public record CreateStockItemRequest(Guid SellableItemId, InventoryItemType SellableItemType, int InitialQuantity);
