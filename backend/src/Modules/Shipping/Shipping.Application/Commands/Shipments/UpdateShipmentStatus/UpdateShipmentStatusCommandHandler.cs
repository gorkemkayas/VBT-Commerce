using MediatR;
using Shipping.Application.Abstractions;
using Shipping.Domain.Exceptions;

namespace Shipping.Application.Commands.Shipments.UpdateShipmentStatus;

public class UpdateShipmentStatusCommandHandler(IShippingDbContext dbContext) : IRequestHandler<UpdateShipmentStatusCommand, Unit>
{
    public async Task<Unit> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var shipment = await dbContext.Shipments.FindAsync([request.ShipmentId], cancellationToken)
            ?? throw new ShipmentNotFoundException(request.ShipmentId);

        shipment.UpdateStatus(request.Status, request.TrackingNumber);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
