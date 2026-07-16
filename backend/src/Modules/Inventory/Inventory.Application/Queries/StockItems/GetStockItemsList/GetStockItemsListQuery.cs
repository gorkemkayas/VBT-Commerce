using BuildingBlocks.Application.Messaging;
using Inventory.Application.Common;
using Inventory.Domain.Enums;

namespace Inventory.Application.Queries.StockItems.GetStockItemsList;

public record GetStockItemsListQuery(
    int PageNumber = 1,
    int PageSize = 20,
    InventoryItemType? SellableItemType = null) : IQuery<PagedResult<StockItemDto>>;
