using Inventory.Application.Abstractions;
using Inventory.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Commands.Reservations.ReserveStock;

public class ReserveStockCommandHandler(IInventoryDbContext dbContext) : IRequestHandler<ReserveStockCommand, Unit>
{
    private static readonly TimeSpan ReservationWindow = TimeSpan.FromMinutes(30);

    public async Task<Unit> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        var expiresAt = DateTime.UtcNow.Add(ReservationWindow);

        foreach (var item in request.Items)
        {
            var stockItem = await dbContext.StockItems
                .Include(s => s.Reservations)
                .FirstOrDefaultAsync(
                    s => s.SellableItemId == item.SellableItemId && s.SellableItemType == item.SellableItemType,
                    cancellationToken)
                ?? throw new StockItemNotFoundException(item.SellableItemId, item.SellableItemType);

            var reservation = stockItem.Reserve(request.ReferenceId, item.Quantity, expiresAt);
            dbContext.StockReservations.Add(reservation);
        }

        // Single SaveChanges at the end: if any item above threw, nothing here has been persisted yet,
        // so the whole batch is all-or-nothing (TransactionBehavior wraps this in a DB transaction too).
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
