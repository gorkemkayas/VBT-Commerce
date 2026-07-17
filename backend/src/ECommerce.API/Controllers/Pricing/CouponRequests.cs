using Pricing.Domain.Enums;

namespace ECommerce.API.Controllers.Pricing;

public record CreateCouponRequest(
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
    int? PerUserUsageLimit);

public record UpdateCouponRequest(
    CouponDiscountType DiscountType,
    decimal DiscountValue,
    decimal? MaxDiscountAmount,
    decimal? MinCartAmount,
    CouponScopeType ScopeType,
    Guid? ScopeReferenceId,
    DateTime StartDate,
    DateTime EndDate,
    int? TotalUsageLimit,
    int? PerUserUsageLimit);
