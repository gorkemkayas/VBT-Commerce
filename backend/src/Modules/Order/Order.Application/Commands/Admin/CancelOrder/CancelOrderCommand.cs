using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Order.Application.Commands.Admin.CancelOrder;

/// <summary>
/// Temporary stand-in for a real payment-failure webhook until the Payment module exists, and also
/// covers admin-initiated cancellation of a pending order. Admin-only.
/// </summary>
public record CancelOrderCommand(Guid OrderId, string? Reason) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
