using Inventory.Application.Abstractions;
using Inventory.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Commands.Reservations.ReleaseReservationsByReference;

public class ReleaseReservationsByReferenceCommandHandler(IInventoryDbContext dbContext)
    : IRequestHandler<ReleaseReservationsByReferenceCommand, Unit>
{
    public async Task<Unit> Handle(ReleaseReservationsByReferenceCommand request, CancellationToken cancellationToken)
    {
        var allReservations = await dbContext.StockReservations
            .Where(r => r.ReferenceId == request.ReferenceId)
            .ToListAsync(cancellationToken);

        // Unknown reference: nothing was ever reserved under this id — treat as a harmless idempotent no-op.
        if (allReservations.Count == 0)
            return Unit.Value;

        var activeReservations = allReservations.Where(r => !r.IsConfirmed && !r.IsReleased).ToList();

        // Known reference, but every reservation under it is already confirmed/released — releasing
        // an already-confirmed (i.e. already consumed) reservation would silently do nothing, which
        // more likely means caller misuse than a legitimate retry, so it's surfaced instead of swallowed.
        if (activeReservations.Count == 0)
            throw new NoActiveReservationsForReferenceException(request.ReferenceId);

        var stockItemIds = activeReservations.Select(r => r.StockItemId).Distinct().ToList();

        var stockItems = await dbContext.StockItems
            .Include(s => s.Reservations)
            .Where(s => stockItemIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        foreach (var stockItem in stockItems)
        {
            var reservationIds = stockItem.Reservations
                .Where(r => r.ReferenceId == request.ReferenceId && !r.IsConfirmed && !r.IsReleased)
                .Select(r => r.Id)
                .ToList();

            foreach (var reservationId in reservationIds)
                stockItem.ReleaseReservation(reservationId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
