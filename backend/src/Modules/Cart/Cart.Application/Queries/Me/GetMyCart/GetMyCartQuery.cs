using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Cart.Application.Common;

namespace Cart.Application.Queries.Me.GetMyCart;

public record GetMyCartQuery : IQuery<CartDto>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
