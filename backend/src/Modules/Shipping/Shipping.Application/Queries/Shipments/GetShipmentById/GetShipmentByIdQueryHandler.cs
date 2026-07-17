using MediatR;
using Shipping.Application.Abstractions;
using Shipping.Application.Common;
using Shipping.Domain.Exceptions;

namespace Shipping.Application.Queries.Shipments.GetShipmentById;

public class GetShipmentByIdQueryHandler(IShippingDbContext dbContext) : IRequestHandler<GetShipmentByIdQuery, ShipmentDto>
{
    public async Task<ShipmentDto> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var shipment = await dbContext.Shipments.FindAsync([request.ShipmentId], cancellationToken)
            ?? throw new ShipmentNotFoundException(request.ShipmentId);

        return ShipmentMapper.ToDto(shipment);
    }
}
