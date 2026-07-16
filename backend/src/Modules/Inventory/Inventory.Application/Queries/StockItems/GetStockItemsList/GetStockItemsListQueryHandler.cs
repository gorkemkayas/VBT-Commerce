using Inventory.Application.Abstractions;
using Inventory.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Queries.StockItems.GetStockItemsList;

public class GetStockItemsListQueryHandler(IInventoryDbContext dbContext)
    : IRequestHandler<GetStockItemsListQuery, PagedResult<StockItemDto>>
{
    public async Task<PagedResult<StockItemDto>> Handle(GetStockItemsListQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var query = dbContext.StockItems.AsNoTracking().AsQueryable();

        if (request.SellableItemType is not null)
            query = query.Where(s => s.SellableItemType == request.SellableItemType);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.SellableItemType).ThenBy(s => s.SellableItemId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StockItemDto(
                s.Id,
                s.SellableItemId,
                s.SellableItemType,
                s.QuantityOnHand,
                s.QuantityOnHand - s.Reservations
                    .Where(r => !r.IsConfirmed && !r.IsReleased && r.ExpiresAt > now)
                    .Sum(r => (int?)r.Quantity) ?? 0))
            .ToListAsync(cancellationToken);

        return new PagedResult<StockItemDto>(items, request.PageNumber, request.PageSize, totalCount);
    }
}
