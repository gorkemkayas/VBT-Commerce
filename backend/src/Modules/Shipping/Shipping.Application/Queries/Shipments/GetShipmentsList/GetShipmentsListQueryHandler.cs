using MediatR;
using Microsoft.EntityFrameworkCore;
using Shipping.Application.Abstractions;
using Shipping.Application.Common;

namespace Shipping.Application.Queries.Shipments.GetShipmentsList;

public class GetShipmentsListQueryHandler(IShippingDbContext dbContext)
    : IRequestHandler<GetShipmentsListQuery, PagedResult<ShipmentDto>>
{
    public async Task<PagedResult<ShipmentDto>> Handle(GetShipmentsListQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Shipments.AsNoTracking();

        if (request.Status is not null)
            query = query.Where(s => s.Status == request.Status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ShipmentDto>(
            items.Select(ShipmentMapper.ToDto).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
