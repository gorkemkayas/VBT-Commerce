using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Inventory.Application.Common;
using Inventory.Domain.Enums;

namespace Inventory.Application.Queries.Reservations.GetStockReservationsList;

public record GetStockReservationsListQuery(
    Guid? SellableItemId = null,
    InventoryItemType? SellableItemType = null,
    bool? IsConfirmed = null,
    bool? IsReleased = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResult<StockReservationDto>>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
