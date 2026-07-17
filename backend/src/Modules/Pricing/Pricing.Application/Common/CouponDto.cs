using Pricing.Domain.Enums;

namespace Pricing.Application.Common;

public record CouponDto(
    Guid Id,
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
    int? PerUserUsageLimit,
    bool IsActive);
