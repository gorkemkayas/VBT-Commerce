using Inventory.Domain.Enums;

namespace ECommerce.API.Controllers.Inventory.Requests;

public record CreateStockItemRequest(Guid SellableItemId, InventoryItemType SellableItemType, int InitialQuantity);
