using Inventory.Domain.Enums;

namespace Inventory.Application.Common;

public record StockItemDto(
    Guid Id,
    Guid SellableItemId,
    InventoryItemType SellableItemType,
    int QuantityOnHand,
    int AvailableQuantity);

public record StockReservationDto(
    Guid Id,
    Guid StockItemId,
    Guid ReferenceId,
    int Quantity,
    DateTime ExpiresAt,
    bool IsConfirmed,
    bool IsReleased);
