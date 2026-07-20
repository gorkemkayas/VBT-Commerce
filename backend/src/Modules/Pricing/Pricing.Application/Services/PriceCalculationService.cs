using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;
using Pricing.Application.Integrations;
using Pricing.Contracts;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;

namespace Pricing.Application.Services;

/// <summary>
/// Shared price/tax/coupon calculation logic used by both the self-service ("me") and guest checkout
/// query handlers — mirrors Cart's CartOperations. Pricing never reads a cart itself: the caller
/// supplies the line items directly (today the frontend, later Order).
/// </summary>
public class PriceCalculationService(
    IPricingDbContext dbContext,
    ICatalogIntegrationService catalogIntegrationService,
    ICustomerIntegrationService customerIntegrationService)
{
    public async Task<PriceCalculationResultDto> CalculateAsync(
        PricingOwnerKey owner,
        IReadOnlyCollection<PriceCalculationItem> items,
        IReadOnlyCollection<string> couponCodes,
        CancellationToken cancellationToken)
    {
        if (owner.GuestCustomerId is not null)
        {
            var guestExists = await customerIntegrationService.GuestCustomerExistsAsync(owner.GuestCustomerId.Value, cancellationToken);
            if (!guestExists)
                throw new PricingGuestCustomerNotFoundException(owner.GuestCustomerId.Value);
        }

        var lines = new List<PriceCalculationLineDto>();
        foreach (var item in items)
        {
            var price = await dbContext.Prices.FirstOrDefaultAsync(
                p => p.SellableItemId == item.SellableItemId && p.SellableItemType == item.SellableItemType,
                cancellationToken)
                ?? throw new PriceNotFoundException(item.SellableItemId, item.SellableItemType);

            lines.Add(new PriceCalculationLineDto(item.SellableItemId, item.SellableItemType, item.Quantity, price.Amount, price.Amount * item.Quantity));
        }

        var subtotal = lines.Sum(l => l.LineSubtotal);

        var appliedCoupons = new List<AppliedCouponDto>();
        var categoryCache = new Dictionary<(Guid, PriceItemType), Guid?>();

        foreach (var code in couponCodes.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(
                c => c.Code == code, cancellationToken)
                ?? throw CouponNotFoundException.ForCode(code);

            await ValidateCouponAsync(coupon, owner, subtotal, cancellationToken);

            var scopedSubtotal = await GetScopedSubtotalAsync(coupon, lines, categoryCache, cancellationToken);

            var discount = coupon.DiscountType == CouponDiscountType.Percentage
                ? scopedSubtotal * coupon.DiscountValue / 100m
                : coupon.DiscountValue;

            discount = Math.Min(discount, scopedSubtotal);
            if (coupon.MaxDiscountAmount is not null)
                discount = Math.Min(discount, coupon.MaxDiscountAmount.Value);

            appliedCoupons.Add(new AppliedCouponDto(coupon.Code, discount));
        }

        var rawTotalDiscount = appliedCoupons.Sum(c => c.DiscountAmount);
        if (rawTotalDiscount > subtotal && rawTotalDiscount > 0)
        {
            var scaleFactor = subtotal / rawTotalDiscount;
            appliedCoupons = appliedCoupons.Select(c => c with { DiscountAmount = c.DiscountAmount * scaleFactor }).ToList();
        }

        var totalDiscount = Math.Min(appliedCoupons.Sum(c => c.DiscountAmount), subtotal);
        var discountedSubtotal = subtotal - totalDiscount;

        var taxRate = await dbContext.TaxRates.FirstAsync(t => t.Id == TaxRate.SingletonId, cancellationToken);
        var taxAmount = discountedSubtotal * taxRate.Rate / 100m;
        var grandTotal = discountedSubtotal + taxAmount;

        return new PriceCalculationResultDto(lines, subtotal, appliedCoupons, totalDiscount, taxRate.Rate, taxAmount, grandTotal);
    }

    private async Task ValidateCouponAsync(Coupon coupon, PricingOwnerKey owner, decimal subtotal, CancellationToken cancellationToken)
    {
        if (!coupon.IsActive)
            throw new CouponInactiveException(coupon.Code);

        var now = DateTime.UtcNow;
        if (now < coupon.StartDate)
            throw new CouponNotYetActiveException(coupon.Code);
        if (now > coupon.EndDate)
            throw new CouponExpiredException(coupon.Code);

        if (coupon.MinCartAmount is not null && subtotal < coupon.MinCartAmount.Value)
            throw new CouponMinCartAmountNotMetException(coupon.Code, coupon.MinCartAmount.Value);

        if (coupon.TotalUsageLimit is not null)
        {
            var totalUsageCount = await dbContext.CouponUsages.CountAsync(u => u.CouponId == coupon.Id, cancellationToken);
            if (totalUsageCount >= coupon.TotalUsageLimit.Value)
                throw CouponUsageLimitExceededException.ForTotalLimit(coupon.Code);
        }

        if (coupon.PerUserUsageLimit is not null)
        {
            var perUserUsageCount = owner.CustomerId != null
                ? await dbContext.CouponUsages.CountAsync(u => u.CouponId == coupon.Id && u.CustomerId == owner.CustomerId, cancellationToken)
                : await dbContext.CouponUsages.CountAsync(u => u.CouponId == coupon.Id && u.GuestCustomerId == owner.GuestCustomerId, cancellationToken);

            if (perUserUsageCount >= coupon.PerUserUsageLimit.Value)
                throw CouponUsageLimitExceededException.ForPerUserLimit(coupon.Code);
        }
    }

    private async Task<decimal> GetScopedSubtotalAsync(
        Coupon coupon,
        List<PriceCalculationLineDto> lines,
        Dictionary<(Guid, PriceItemType), Guid?> categoryCache,
        CancellationToken cancellationToken)
    {
        switch (coupon.ScopeType)
        {
            case CouponScopeType.Cart:
                return lines.Sum(l => l.LineSubtotal);

            case CouponScopeType.Product:
                return lines.Where(l => l.SellableItemId == coupon.ScopeReferenceId).Sum(l => l.LineSubtotal);

            case CouponScopeType.Category:
                decimal scoped = 0;
                foreach (var line in lines)
                {
                    var key = (line.SellableItemId, line.SellableItemType);
                    if (!categoryCache.TryGetValue(key, out var categoryId))
                    {
                        categoryId = await catalogIntegrationService.GetCategoryIdAsync(line.SellableItemId, line.SellableItemType, cancellationToken);
                        categoryCache[key] = categoryId;
                    }

                    if (categoryId == coupon.ScopeReferenceId)
                        scoped += line.LineSubtotal;
                }

                return scoped;

            default:
                return 0;
        }
    }
}
