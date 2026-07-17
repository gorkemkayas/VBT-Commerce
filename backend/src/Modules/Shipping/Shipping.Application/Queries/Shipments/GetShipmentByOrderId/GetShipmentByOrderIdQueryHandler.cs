using MediatR;
using Microsoft.EntityFrameworkCore;
using Shipping.Application.Abstractions;
using Shipping.Application.Common;
using Shipping.Domain.Exceptions;

namespace Shipping.Application.Queries.Shipments.GetShipmentByOrderId;

public class GetShipmentByOrderIdQueryHandler(IShippingDbContext dbContext) : IRequestHandler<GetShipmentByOrderIdQuery, ShipmentDto>
{
    public async Task<ShipmentDto> Handle(GetShipmentByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var shipment = await dbContext.Shipments.FirstOrDefaultAsync(s => s.OrderId == request.OrderId, cancellationToken)
            ?? throw new ShipmentNotFoundException(request.OrderId);

        return ShipmentMapper.ToDto(shipment);
    }
}
