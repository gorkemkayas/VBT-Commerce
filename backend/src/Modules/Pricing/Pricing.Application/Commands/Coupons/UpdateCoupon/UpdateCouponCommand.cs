using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;
using Pricing.Domain.Enums;

namespace Pricing.Application.Commands.Coupons.UpdateCoupon;

public record UpdateCouponCommand(
    Guid CouponId,
    CouponDiscountType DiscountType,
    decimal DiscountValue,
    decimal? MaxDiscountAmount,
    decimal? MinCartAmount,
    CouponScopeType ScopeType,
    Guid? ScopeReferenceId,
    DateTime StartDate,
    DateTime EndDate,
    int? TotalUsageLimit,
    int? PerUserUsageLimit) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
