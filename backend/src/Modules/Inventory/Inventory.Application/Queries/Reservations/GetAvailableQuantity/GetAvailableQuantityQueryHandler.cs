using Inventory.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Queries.Reservations.GetAvailableQuantity;

/// <summary>
/// Returns 0 (rather than throwing) when no stock item is tracked yet for the given sellable item —
/// this query is meant for public "is it in stock" checks (Cart, product page), where an untracked
/// item should simply read as unavailable instead of erroring.
/// </summary>
public class GetAvailableQuantityQueryHandler(IInventoryDbContext dbContext) : IRequestHandler<GetAvailableQuantityQuery, int>
{
    public async Task<int> Handle(GetAvailableQuantityQuery request, CancellationToken cancellationToken)
    {
        var stockItem = await dbContext.StockItems
            .AsNoTracking()
            .Include(s => s.Reservations)
            .FirstOrDefaultAsync(
                s => s.SellableItemId == request.SellableItemId && s.SellableItemType == request.SellableItemType,
                cancellationToken);

        return stockItem?.AvailableQuantity(DateTime.UtcNow) ?? 0;
    }
}
