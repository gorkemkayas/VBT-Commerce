using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Inventory.Domain.Enums;

namespace Inventory.Application.Commands.StockItems.CreateStockItem;

public record CreateStockItemCommand(
    Guid SellableItemId,
    InventoryItemType SellableItemType,
    int InitialQuantity) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
