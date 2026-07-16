using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Cart.Application.Commands.Me.UpdateMyCartItemQuantity;

public record UpdateMyCartItemQuantityCommand(Guid CartItemId, int Quantity) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
