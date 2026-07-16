using Inventory.Application.Abstractions;
using Inventory.Application.Common;
using Inventory.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Queries.StockItems.GetStockItemBySellableItem;

public class GetStockItemBySellableItemQueryHandler(IInventoryDbContext dbContext)
    : IRequestHandler<GetStockItemBySellableItemQuery, StockItemDto>
{
    public async Task<StockItemDto> Handle(GetStockItemBySellableItemQuery request, CancellationToken cancellationToken)
    {
        var stockItem = await dbContext.StockItems
            .AsNoTracking()
            .Include(s => s.Reservations)
            .FirstOrDefaultAsync(
                s => s.SellableItemId == request.SellableItemId && s.SellableItemType == request.SellableItemType,
                cancellationToken)
            ?? throw new StockItemNotFoundException(request.SellableItemId, request.SellableItemType);

        var now = DateTime.UtcNow;

        return new StockItemDto(
            stockItem.Id,
            stockItem.SellableItemId,
            stockItem.SellableItemType,
            stockItem.QuantityOnHand,
            stockItem.AvailableQuantity(now));
    }
}
