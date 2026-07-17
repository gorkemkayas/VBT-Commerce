using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Pricing.Domain.Enums;

namespace Pricing.Application.Commands.Coupons.CreateCoupon;

public record CreateCouponCommand(
    string Code,
    CouponDiscountType DiscountType,
    decimal DiscountValue,
    decimal? MaxDiscountAmount,
    decimal? MinCartAmount,
    CouponScopeType ScopeType,
    Guid? ScopeReferenceId,
    DateTime StartDate,
    DateTime EndDate,
    int? TotalUsageLimit,
    int? PerUserUsageLimit) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
