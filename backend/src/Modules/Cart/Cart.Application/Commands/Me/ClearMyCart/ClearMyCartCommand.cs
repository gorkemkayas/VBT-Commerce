using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Cart.Application.Commands.Me.ClearMyCart;

public record ClearMyCartCommand : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
