using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Cart.Application.Commands.Me.RemoveMyCartItem;

public record RemoveMyCartItemCommand(Guid CartItemId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
