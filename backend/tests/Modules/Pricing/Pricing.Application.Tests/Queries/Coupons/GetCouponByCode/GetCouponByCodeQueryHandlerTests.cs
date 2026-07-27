using FluentAssertions;
using Pricing.Application.Queries.Coupons.GetCouponByCode;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;

namespace Pricing.Application.Tests.Queries.Coupons.GetCouponByCode;

public class GetCouponByCodeQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCode_ReturnsCouponDto()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var coupon = Coupon.Create(
            "SAVE10", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null, null);
        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetCouponByCodeQueryHandler(dbContext);

        var result = await handler.Handle(new GetCouponByCodeQuery("SAVE10"), CancellationToken.None);

        result.Id.Should().Be(coupon.Id);
        result.Code.Should().Be("SAVE10");
    }

    [Fact]
    public async Task Handle_WithUnknownCode_ThrowsCouponNotFoundException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var handler = new GetCouponByCodeQueryHandler(dbContext);

        await Assert.ThrowsAsync<CouponNotFoundException>(
            () => handler.Handle(new GetCouponByCodeQuery("MISSING"), CancellationToken.None));
    }
}
