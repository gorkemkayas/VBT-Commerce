using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;

namespace Pricing.Domain.Entities;

public class Coupon
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public CouponDiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public decimal? MaxDiscountAmount { get; private set; }
    public decimal? MinCartAmount { get; private set; }
    public CouponScopeType ScopeType { get; private set; }
    public Guid? ScopeReferenceId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int? TotalUsageLimit { get; private set; }
    public int? PerUserUsageLimit { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Coupon()
    {
    }

    public static Coupon Create(
        string code,
        CouponDiscountType discountType,
        decimal discountValue,
        decimal? maxDiscountAmount,
        decimal? minCartAmount,
        CouponScopeType scopeType,
        Guid? scopeReferenceId,
        DateTime startDate,
        DateTime endDate,
        int? totalUsageLimit,
        int? perUserUsageLimit)
    {
        Validate(discountType, discountValue, scopeType, scopeReferenceId, startDate, endDate);

        return new Coupon
        {
            Id = Guid.NewGuid(),
            Code = code,
            DiscountType = discountType,
            DiscountValue = discountValue,
            MaxDiscountAmount = maxDiscountAmount,
            MinCartAmount = minCartAmount,
            ScopeType = scopeType,
            ScopeReferenceId = scopeReferenceId,
            StartDate = startDate,
            EndDate = endDate,
            TotalUsageLimit = totalUsageLimit,
            PerUserUsageLimit = perUserUsageLimit,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        CouponDiscountType discountType,
        decimal discountValue,
        decimal? maxDiscountAmount,
        decimal? minCartAmount,
        CouponScopeType scopeType,
        Guid? scopeReferenceId,
        DateTime startDate,
        DateTime endDate,
        int? totalUsageLimit,
        int? perUserUsageLimit)
    {
        Validate(discountType, discountValue, scopeType, scopeReferenceId, startDate, endDate);

        DiscountType = discountType;
        DiscountValue = discountValue;
        MaxDiscountAmount = maxDiscountAmount;
        MinCartAmount = minCartAmount;
        ScopeType = scopeType;
        ScopeReferenceId = scopeReferenceId;
        StartDate = startDate;
        EndDate = endDate;
        TotalUsageLimit = totalUsageLimit;
        PerUserUsageLimit = perUserUsageLimit;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(
        CouponDiscountType discountType,
        decimal discountValue,
        CouponScopeType scopeType,
        Guid? scopeReferenceId,
        DateTime startDate,
        DateTime endDate)
    {
        if (discountValue <= 0)
            throw new InvalidCouponConfigurationException("DiscountValue must be greater than zero.");

        if (discountType == CouponDiscountType.Percentage && discountValue > 100)
            throw new InvalidCouponConfigurationException("A percentage DiscountValue cannot exceed 100.");

        if (scopeType != CouponScopeType.Cart && scopeReferenceId is null)
            throw new InvalidCouponConfigurationException("ScopeReferenceId is required for Category or Product scope.");

        if (scopeType == CouponScopeType.Cart && scopeReferenceId is not null)
            throw new InvalidCouponConfigurationException("ScopeReferenceId must be null for Cart scope.");

        if (endDate <= startDate)
            throw new InvalidCouponConfigurationException("EndDate must be after StartDate.");
    }
}
