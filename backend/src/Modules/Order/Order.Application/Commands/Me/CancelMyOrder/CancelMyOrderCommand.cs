using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Order.Application.Commands.Me.CancelMyOrder;

public record CancelMyOrderCommand(Guid OrderId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
