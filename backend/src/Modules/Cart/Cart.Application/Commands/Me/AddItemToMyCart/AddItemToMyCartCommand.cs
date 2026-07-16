using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Cart.Domain.Enums;

namespace Cart.Application.Commands.Me.AddItemToMyCart;

public record AddItemToMyCartCommand(Guid SellableItemId, CartItemType SellableItemType, int Quantity)
    : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
