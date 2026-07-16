using BuildingBlocks.Application.Messaging;
using Inventory.Application.Common;
using Inventory.Domain.Enums;

namespace Inventory.Application.Queries.StockItems.GetStockItemBySellableItem;

public record GetStockItemBySellableItemQuery(Guid SellableItemId, InventoryItemType SellableItemType) : IQuery<StockItemDto>;
