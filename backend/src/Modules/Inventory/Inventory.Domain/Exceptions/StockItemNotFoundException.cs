using BuildingBlocks.Domain;
using Inventory.Domain.Enums;

namespace Inventory.Domain.Exceptions;

public class StockItemNotFoundException : DomainException
{
    public StockItemNotFoundException(Guid stockItemId) : base($"Stock item '{stockItemId}' was not found.")
    {
    }

    public StockItemNotFoundException(Guid sellableItemId, InventoryItemType sellableItemType)
        : base($"No stock item was found for {sellableItemType} '{sellableItemId}'.")
    {
    }
}
