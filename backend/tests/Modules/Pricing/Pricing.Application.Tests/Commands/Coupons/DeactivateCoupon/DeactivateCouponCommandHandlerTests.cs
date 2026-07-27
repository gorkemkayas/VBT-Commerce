using FluentAssertions;
using Pricing.Application.Commands.Coupons.DeactivateCoupon;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;

namespace Pricing.Application.Tests.Commands.Coupons.DeactivateCoupon;

public class DeactivateCouponCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCoupon_DeactivatesIt()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var coupon = Coupon.Create(
            "SAVE10", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null, null);
        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeactivateCouponCommandHandler(dbContext);

        await handler.Handle(new DeactivateCouponCommand(coupon.Id), CancellationToken.None);

        var stored = await dbContext.Coupons.FindAsync(coupon.Id);
        stored!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithNonExistentCoupon_ThrowsCouponNotFoundException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var handler = new DeactivateCouponCommandHandler(dbContext);

        await Assert.ThrowsAsync<CouponNotFoundException>(
            () => handler.Handle(new DeactivateCouponCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
