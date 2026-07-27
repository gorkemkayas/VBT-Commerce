using FluentAssertions;
using Moq;
using Pricing.Application.Integrations;
using Pricing.Application.Queries.Calculate.CalculateCustomerOrderPrice;
using Pricing.Application.Services;
using Pricing.Contracts;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;
using DomainTaxRate = Pricing.Domain.Entities.TaxRate;

namespace Pricing.Application.Tests.Queries.Calculate.CalculateCustomerOrderPrice;

public class CalculateCustomerOrderPriceQueryHandlerTests
{
    private static (Pricing.Infrastructure.Persistence.PricingDbContext dbContext, CalculateCustomerOrderPriceQueryHandler handler)
        CreateHandler(decimal taxRatePercent = 0, Guid? categoryIdForItems = null)
    {
        var dbContext = TestPricingDbContextFactory.Create();
        dbContext.TaxRates.Add(DomainTaxRate.CreateDefault(taxRatePercent));
        dbContext.SaveChangesAsync(CancellationToken.None).GetAwaiter().GetResult();

        var catalogIntegrationServiceMock = new Mock<ICatalogIntegrationService>();
        catalogIntegrationServiceMock
            .Setup(s => s.GetCategoryIdAsync(It.IsAny<Guid>(), It.IsAny<PriceItemType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryIdForItems);

        var customerIntegrationServiceMock = new Mock<ICustomerIntegrationService>();

        var service = new PriceCalculationService(dbContext, catalogIntegrationServiceMock.Object, customerIntegrationServiceMock.Object);
        var handler = new CalculateCustomerOrderPriceQueryHandler(service);

        return (dbContext, handler);
    }

    private static Price AddPrice(Pricing.Infrastructure.Persistence.PricingDbContext dbContext, Guid sellableItemId, decimal amount)
    {
        var price = Price.Create(sellableItemId, PriceItemType.Product, amount);
        dbContext.Prices.Add(price);
        return price;
    }

    private static Coupon AddCoupon(
        Pricing.Infrastructure.Persistence.PricingDbContext dbContext,
        string code,
        CouponDiscountType discountType,
        decimal discountValue,
        decimal? maxDiscountAmount = null,
        decimal? minCartAmount = null,
        CouponScopeType scopeType = CouponScopeType.Cart,
        Guid? scopeReferenceId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? totalUsageLimit = null,
        int? perUserUsageLimit = null)
    {
        var coupon = Coupon.Create(
            code,
            discountType,
            discountValue,
            maxDiscountAmount,
            minCartAmount,
            scopeType,
            scopeReferenceId,
            startDate ?? DateTime.UtcNow.AddDays(-1),
            endDate ?? DateTime.UtcNow.AddDays(30),
            totalUsageLimit,
            perUserUsageLimit);
        dbContext.Coupons.Add(coupon);
        return coupon;
    }

    [Fact]
    public async Task Handle_WithNoCoupons_ComputesSubtotalAndTaxOnly()
    {
        var (dbContext, handler) = CreateHandler(taxRatePercent: 10m);
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 50m);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 2)],
            []);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Subtotal.Should().Be(100m);
        result.TotalDiscount.Should().Be(0m);
        result.TaxRate.Should().Be(10m);
        result.TaxAmount.Should().Be(10m);
        result.GrandTotal.Should().Be(110m);
    }

    [Fact]
    public async Task Handle_WithPercentageCartCoupon_AppliesPercentageDiscount()
    {
        var (dbContext, handler) = CreateHandler(taxRatePercent: 0);
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        AddCoupon(dbContext, "SAVE10", CouponDiscountType.Percentage, 10);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["SAVE10"]);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Subtotal.Should().Be(100m);
        result.TotalDiscount.Should().Be(10m);
        result.GrandTotal.Should().Be(90m);
        result.AppliedCoupons.Single().DiscountAmount.Should().Be(10m);
    }

    [Fact]
    public async Task Handle_WithFixedAmountCoupon_AppliesFixedDiscount()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        AddCoupon(dbContext, "FLAT20", CouponDiscountType.FixedAmount, 20);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["FLAT20"]);

        var result = await handler.Handle(query, CancellationToken.None);

        result.TotalDiscount.Should().Be(20m);
        result.GrandTotal.Should().Be(80m);
    }

    [Fact]
    public async Task Handle_WithFixedDiscountExceedingSubtotal_CapsDiscountAtSubtotal()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 10m);
        AddCoupon(dbContext, "FLAT20", CouponDiscountType.FixedAmount, 20);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["FLAT20"]);

        var result = await handler.Handle(query, CancellationToken.None);

        result.TotalDiscount.Should().Be(10m);
        result.GrandTotal.Should().Be(0m);
    }

    [Fact]
    public async Task Handle_WithMaxDiscountAmount_CapsPercentageDiscount()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 1000m);
        AddCoupon(dbContext, "SAVE50", CouponDiscountType.Percentage, 50, maxDiscountAmount: 30m);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["SAVE50"]);

        var result = await handler.Handle(query, CancellationToken.None);

        result.TotalDiscount.Should().Be(30m);
        result.GrandTotal.Should().Be(970m);
    }

    [Fact]
    public async Task Handle_WithProductScopedCoupon_OnlyDiscountsMatchingLine()
    {
        var (dbContext, handler) = CreateHandler();
        var matchingItemId = Guid.NewGuid();
        var otherItemId = Guid.NewGuid();
        AddPrice(dbContext, matchingItemId, 50m);
        AddPrice(dbContext, otherItemId, 50m);
        AddCoupon(dbContext, "PRODUCT10", CouponDiscountType.Percentage, 10, scopeType: CouponScopeType.Product, scopeReferenceId: matchingItemId);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [
                new PriceCalculationItem(matchingItemId, PriceItemType.Product, 1),
                new PriceCalculationItem(otherItemId, PriceItemType.Product, 1)
            ],
            ["PRODUCT10"]);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Subtotal.Should().Be(100m);
        result.TotalDiscount.Should().Be(5m);
    }

    [Fact]
    public async Task Handle_WithCategoryScopedCoupon_OnlyDiscountsItemsInThatCategory()
    {
        var categoryId = Guid.NewGuid();
        var (dbContext, handler) = CreateHandler(categoryIdForItems: categoryId);
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        AddCoupon(dbContext, "CAT10", CouponDiscountType.Percentage, 10, scopeType: CouponScopeType.Category, scopeReferenceId: categoryId);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["CAT10"]);

        var result = await handler.Handle(query, CancellationToken.None);

        result.TotalDiscount.Should().Be(10m);
    }

    [Fact]
    public async Task Handle_WithMultipleCouponsExceedingSubtotal_ScalesDiscountsProportionally()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        AddCoupon(dbContext, "FLAT60A", CouponDiscountType.FixedAmount, 60);
        AddCoupon(dbContext, "FLAT60B", CouponDiscountType.FixedAmount, 60);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["FLAT60A", "FLAT60B"]);

        var result = await handler.Handle(query, CancellationToken.None);

        result.TotalDiscount.Should().Be(100m);
        result.GrandTotal.Should().Be(0m);
        result.AppliedCoupons.Sum(c => c.DiscountAmount).Should().Be(100m);
    }

    [Fact]
    public async Task Handle_WithTaxRateApplied_ComputesTaxOnDiscountedSubtotal()
    {
        var (dbContext, handler) = CreateHandler(taxRatePercent: 20m);
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        AddCoupon(dbContext, "SAVE10", CouponDiscountType.Percentage, 10);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["SAVE10"]);

        var result = await handler.Handle(query, CancellationToken.None);

        // discountedSubtotal = 90, tax = 90 * 20% = 18, grandTotal = 108
        result.TaxAmount.Should().Be(18m);
        result.GrandTotal.Should().Be(108m);
    }

    [Fact]
    public async Task Handle_WithUnknownItemPrice_ThrowsPriceNotFoundException()
    {
        var (dbContext, handler) = CreateHandler();
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(Guid.NewGuid(), PriceItemType.Product, 1)],
            []);

        await Assert.ThrowsAsync<PriceNotFoundException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithUnknownCouponCode_ThrowsCouponNotFoundException()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["UNKNOWN"]);

        await Assert.ThrowsAsync<CouponNotFoundException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInactiveCoupon_ThrowsCouponInactiveException()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        var coupon = AddCoupon(dbContext, "SAVE10", CouponDiscountType.Percentage, 10);
        coupon.Deactivate();
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["SAVE10"]);

        await Assert.ThrowsAsync<CouponInactiveException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNotYetActiveCoupon_ThrowsCouponNotYetActiveException()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        AddCoupon(dbContext, "FUTURE10", CouponDiscountType.Percentage, 10,
            startDate: DateTime.UtcNow.AddDays(5), endDate: DateTime.UtcNow.AddDays(10));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["FUTURE10"]);

        await Assert.ThrowsAsync<CouponNotYetActiveException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithExpiredCoupon_ThrowsCouponExpiredException()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        AddCoupon(dbContext, "OLD10", CouponDiscountType.Percentage, 10,
            startDate: DateTime.UtcNow.AddDays(-30), endDate: DateTime.UtcNow.AddDays(-1));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["OLD10"]);

        await Assert.ThrowsAsync<CouponExpiredException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithSubtotalBelowMinCartAmount_ThrowsCouponMinCartAmountNotMetException()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 10m);
        AddCoupon(dbContext, "MIN100", CouponDiscountType.Percentage, 10, minCartAmount: 100m);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["MIN100"]);

        await Assert.ThrowsAsync<CouponMinCartAmountNotMetException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithTotalUsageLimitReached_ThrowsCouponUsageLimitExceededException()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        var coupon = AddCoupon(dbContext, "LIMIT1", CouponDiscountType.Percentage, 10, totalUsageLimit: 1);
        dbContext.CouponUsages.Add(Pricing.Domain.Entities.CouponUsage.Create(coupon.Id, Guid.NewGuid(), null, Guid.NewGuid(), 5m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["LIMIT1"]);

        await Assert.ThrowsAsync<CouponUsageLimitExceededException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithPerUserUsageLimitReachedForThisCustomer_ThrowsCouponUsageLimitExceededException()
    {
        var (dbContext, handler) = CreateHandler();
        var customerId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        var coupon = AddCoupon(dbContext, "PERUSER1", CouponDiscountType.Percentage, 10, perUserUsageLimit: 1);
        dbContext.CouponUsages.Add(Pricing.Domain.Entities.CouponUsage.Create(coupon.Id, customerId, null, Guid.NewGuid(), 5m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            customerId,
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["PERUSER1"]);

        await Assert.ThrowsAsync<CouponUsageLimitExceededException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithPerUserUsageLimitReachedByDifferentCustomer_DoesNotThrow()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        var coupon = AddCoupon(dbContext, "PERUSER1", CouponDiscountType.Percentage, 10, perUserUsageLimit: 1);
        dbContext.CouponUsages.Add(Pricing.Domain.Entities.CouponUsage.Create(coupon.Id, Guid.NewGuid(), null, Guid.NewGuid(), 5m));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["PERUSER1"]);

        var result = await handler.Handle(query, CancellationToken.None);

        result.TotalDiscount.Should().Be(10m);
    }

    [Fact]
    public async Task Handle_WithDuplicateCouponCodesDifferentCasing_AppliesOnlyOnce()
    {
        var (dbContext, handler) = CreateHandler();
        var itemId = Guid.NewGuid();
        AddPrice(dbContext, itemId, 100m);
        AddCoupon(dbContext, "SAVE10", CouponDiscountType.Percentage, 10);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new CalculateCustomerOrderPriceQuery(
            Guid.NewGuid(),
            [new PriceCalculationItem(itemId, PriceItemType.Product, 1)],
            ["SAVE10", "save10"]);

        var result = await handler.Handle(query, CancellationToken.None);

        result.AppliedCoupons.Should().HaveCount(1);
        result.TotalDiscount.Should().Be(10m);
    }
}
