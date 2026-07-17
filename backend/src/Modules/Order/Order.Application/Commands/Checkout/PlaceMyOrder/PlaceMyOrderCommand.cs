using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;

namespace Order.Application.Commands.Checkout.PlaceMyOrder;

public record PlaceMyOrderCommand(Guid AddressId, Guid ShippingCompanyId, IReadOnlyCollection<string> CouponCodes)
    : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
