using Inventory.Application.Abstractions;
using Inventory.Application.Common;
using Inventory.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Queries.StockItems.GetStockItemById;

public class GetStockItemByIdQueryHandler(IInventoryDbContext dbContext) : IRequestHandler<GetStockItemByIdQuery, StockItemDto>
{
    public async Task<StockItemDto> Handle(GetStockItemByIdQuery request, CancellationToken cancellationToken)
    {
        var stockItem = await dbContext.StockItems
            .AsNoTracking()
            .Include(s => s.Reservations)
            .FirstOrDefaultAsync(s => s.Id == request.StockItemId, cancellationToken)
            ?? throw new StockItemNotFoundException(request.StockItemId);

        var now = DateTime.UtcNow;

        return new StockItemDto(
            stockItem.Id,
            stockItem.SellableItemId,
            stockItem.SellableItemType,
            stockItem.QuantityOnHand,
            stockItem.AvailableQuantity(now));
    }
}
