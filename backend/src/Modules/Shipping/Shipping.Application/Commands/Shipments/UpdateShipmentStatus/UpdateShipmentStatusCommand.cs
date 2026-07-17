using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;
using Shipping.Domain.Enums;

namespace Shipping.Application.Commands.Shipments.UpdateShipmentStatus;

public record UpdateShipmentStatusCommand(Guid ShipmentId, ShipmentStatus Status, string? TrackingNumber) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
