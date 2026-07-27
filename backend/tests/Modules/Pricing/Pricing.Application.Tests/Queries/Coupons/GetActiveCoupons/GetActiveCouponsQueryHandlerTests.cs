using FluentAssertions;
using Pricing.Application.Queries.Coupons.GetActiveCoupons;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Xunit;

namespace Pricing.Application.Tests.Queries.Coupons.GetActiveCoupons;

public class GetActiveCouponsQueryHandlerTests
{
    private static Coupon MakeCoupon(string code, bool isActive, DateTime startDate, DateTime endDate)
    {
        var coupon = Coupon.Create(
            code, CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            startDate, endDate, null, null);
        if (!isActive)
            coupon.Deactivate();
        return coupon;
    }

    [Fact]
    public async Task Handle_ReturnsOnlyActiveCouponsWithinDateWindow()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var now = DateTime.UtcNow;
        var current = MakeCoupon("CURRENT", true, now.AddDays(-1), now.AddDays(1));
        var expired = MakeCoupon("EXPIRED", true, now.AddDays(-10), now.AddDays(-1));
        var future = MakeCoupon("FUTURE", true, now.AddDays(1), now.AddDays(10));
        var inactive = MakeCoupon("INACTIVE", false, now.AddDays(-1), now.AddDays(1));
        dbContext.Coupons.AddRange(current, expired, future, inactive);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetActiveCouponsQueryHandler(dbContext);

        var result = await handler.Handle(new GetActiveCouponsQuery(), CancellationToken.None);

        result.Should().ContainSingle();
        result.Single().Code.Should().Be("CURRENT");
    }

    [Fact]
    public async Task Handle_WithNoActiveCoupons_ReturnsEmptyList()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var handler = new GetActiveCouponsQueryHandler(dbContext);

        var result = await handler.Handle(new GetActiveCouponsQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
