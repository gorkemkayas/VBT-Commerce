using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Shipping.Application.Common;
using Shipping.Domain.Enums;

namespace Shipping.Application.Queries.Shipments.GetShipmentsList;

public record GetShipmentsListQuery(ShipmentStatus? Status = null, int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResult<ShipmentDto>>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
