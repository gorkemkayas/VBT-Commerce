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

        // StatusHistory's Id is generated client-side, so EF's added-vs-modified heuristic
        // (based on whether the key already has a value) misreads the new entry as an existing
        // row and issues an UPDATE instead of an INSERT — track it explicitly as Added.
        dbContext.ShipmentStatusHistories.Add(shipment.StatusHistory.Last());

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
