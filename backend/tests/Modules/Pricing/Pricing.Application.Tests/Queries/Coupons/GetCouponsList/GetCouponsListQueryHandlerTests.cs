using FluentAssertions;
using Pricing.Application.Queries.Coupons.GetCouponsList;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Xunit;

namespace Pricing.Application.Tests.Queries.Coupons.GetCouponsList;

public class GetCouponsListQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPagedResultWithCorrectTotalCount()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        for (var i = 0; i < 5; i++)
        {
            dbContext.Coupons.Add(Coupon.Create(
                $"CODE{i}", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null, null));
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetCouponsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetCouponsListQuery(1, 2), CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Items.Should().HaveCount(2);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WithNoCoupons_ReturnsEmptyPagedResult()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var handler = new GetCouponsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetCouponsListQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithSecondPage_SkipsFirstPageItems()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        for (var i = 0; i < 3; i++)
        {
            dbContext.Coupons.Add(Coupon.Create(
                $"CODE{i}", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
                DateTime.UtcNow.AddSeconds(i), DateTime.UtcNow.AddDays(30), null, null));
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetCouponsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetCouponsListQuery(2, 2), CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }
}
