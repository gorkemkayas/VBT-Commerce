using Inventory.Application.Abstractions;
using Inventory.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Queries.Reservations.GetStockReservationsList;

public class GetStockReservationsListQueryHandler(IInventoryDbContext dbContext)
    : IRequestHandler<GetStockReservationsListQuery, PagedResult<StockReservationDto>>
{
    public async Task<PagedResult<StockReservationDto>> Handle(GetStockReservationsListQuery request, CancellationToken cancellationToken)
    {
        var query =
            from reservation in dbContext.StockReservations.AsNoTracking()
            join stockItem in dbContext.StockItems.AsNoTracking() on reservation.StockItemId equals stockItem.Id
            select new { Reservation = reservation, StockItem = stockItem };

        if (request.SellableItemId is not null)
            query = query.Where(x => x.StockItem.SellableItemId == request.SellableItemId);

        if (request.SellableItemType is not null)
            query = query.Where(x => x.StockItem.SellableItemType == request.SellableItemType);

        if (request.IsConfirmed is not null)
            query = query.Where(x => x.Reservation.IsConfirmed == request.IsConfirmed);

        if (request.IsReleased is not null)
            query = query.Where(x => x.Reservation.IsReleased == request.IsReleased);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Reservation.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new StockReservationDto(
                x.Reservation.Id,
                x.Reservation.StockItemId,
                x.StockItem.SellableItemId,
                x.StockItem.SellableItemType,
                x.Reservation.ReferenceId,
                x.Reservation.Quantity,
                x.Reservation.ExpiresAt,
                x.Reservation.IsConfirmed,
                x.Reservation.IsReleased,
                x.Reservation.CreatedAt,
                x.Reservation.ConfirmedAt,
                x.Reservation.ReleasedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<StockReservationDto>(items, request.PageNumber, request.PageSize, totalCount);
    }
}
