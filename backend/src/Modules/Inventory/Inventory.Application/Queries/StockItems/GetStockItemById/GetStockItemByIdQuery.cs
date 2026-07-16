using BuildingBlocks.Application.Messaging;
using Inventory.Application.Common;

namespace Inventory.Application.Queries.StockItems.GetStockItemById;

public record GetStockItemByIdQuery(Guid StockItemId) : IQuery<StockItemDto>;
