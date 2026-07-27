using FluentAssertions;
using Pricing.Application.Commands.Coupons.UpdateCoupon;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;

namespace Pricing.Application.Tests.Commands.Coupons.UpdateCoupon;

public class UpdateCouponCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCoupon_UpdatesItsFields()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var coupon = Coupon.Create(
            "SAVE10", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null, null);
        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateCouponCommandHandler(dbContext);
        var command = new UpdateCouponCommand(
            coupon.Id,
            CouponDiscountType.FixedAmount,
            25,
            null,
            null,
            CouponScopeType.Cart,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(60),
            50,
            2);

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Coupons.FindAsync(coupon.Id);
        stored!.DiscountType.Should().Be(CouponDiscountType.FixedAmount);
        stored.DiscountValue.Should().Be(25);
        stored.TotalUsageLimit.Should().Be(50);
        stored.PerUserUsageLimit.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithNonExistentCoupon_ThrowsCouponNotFoundException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var handler = new UpdateCouponCommandHandler(dbContext);
        var command = new UpdateCouponCommand(
            Guid.NewGuid(),
            CouponDiscountType.Percentage,
            10,
            null,
            null,
            CouponScopeType.Cart,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            null,
            null);

        await Assert.ThrowsAsync<CouponNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInvalidConfiguration_ThrowsInvalidCouponConfigurationException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var coupon = Coupon.Create(
            "SAVE10", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null, null);
        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateCouponCommandHandler(dbContext);
        var command = new UpdateCouponCommand(
            coupon.Id,
            CouponDiscountType.Percentage,
            150,
            null,
            null,
            CouponScopeType.Cart,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            null,
            null);

        await Assert.ThrowsAsync<InvalidCouponConfigurationException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
