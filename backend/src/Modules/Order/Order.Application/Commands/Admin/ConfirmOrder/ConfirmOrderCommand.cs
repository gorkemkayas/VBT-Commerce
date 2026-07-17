using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Order.Application.Commands.Admin.ConfirmOrder;

/// <summary>
/// Temporary stand-in for a real payment-success webhook until the Payment module exists — mirrors
/// Shipping's manual delivery-status update. Admin-only for now.
/// </summary>
public record ConfirmOrderCommand(Guid OrderId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
