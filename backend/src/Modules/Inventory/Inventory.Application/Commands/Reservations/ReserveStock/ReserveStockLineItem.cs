using Inventory.Domain.Enums;

namespace Inventory.Application.Commands.Reservations.ReserveStock;

public record ReserveStockLineItem(Guid SellableItemId, InventoryItemType SellableItemType, int Quantity);
