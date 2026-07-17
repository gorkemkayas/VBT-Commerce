using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Pricing.Application.Commands.Coupons.DeactivateCoupon;

public record DeactivateCouponCommand(Guid CouponId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
