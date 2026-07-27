using FluentAssertions;
using Pricing.Application.Commands.CouponUsage.CommitCouponUsage;
using Pricing.Contracts;
using Pricing.Domain.Entities;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;

namespace Pricing.Application.Tests.Commands.CouponUsage.CommitCouponUsage;

public class CommitCouponUsageCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCouponAndCustomer_RecordsUsage()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var coupon = Coupon.Create(
            "SAVE10", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null, null);
        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CommitCouponUsageCommandHandler(dbContext);
        var customerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var command = new CommitCouponUsageCommand(
            [new AppliedCouponDto("SAVE10", 5m)],
            customerId,
            null,
            orderId);

        await handler.Handle(command, CancellationToken.None);

        var usage = dbContext.CouponUsages.Single();
        usage.CouponId.Should().Be(coupon.Id);
        usage.CustomerId.Should().Be(customerId);
        usage.GuestCustomerId.Should().BeNull();
        usage.OrderId.Should().Be(orderId);
        usage.DiscountAmount.Should().Be(5m);
    }

    [Fact]
    public async Task Handle_WithValidCouponAndGuestCustomer_RecordsUsage()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var coupon = Coupon.Create(
            "SAVE10", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null, null);
        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CommitCouponUsageCommandHandler(dbContext);
        var guestCustomerId = Guid.NewGuid();
        var command = new CommitCouponUsageCommand(
            [new AppliedCouponDto("SAVE10", 5m)],
            null,
            guestCustomerId,
            Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var usage = dbContext.CouponUsages.Single();
        usage.GuestCustomerId.Should().Be(guestCustomerId);
        usage.CustomerId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithMultipleAppliedCoupons_RecordsUsageForEach()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var coupon1 = Coupon.Create(
            "SAVE10", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null, null);
        var coupon2 = Coupon.Create(
            "SAVE5", CouponDiscountType.FixedAmount, 5, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null, null);
        dbContext.Coupons.AddRange(coupon1, coupon2);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CommitCouponUsageCommandHandler(dbContext);
        var command = new CommitCouponUsageCommand(
            [new AppliedCouponDto("SAVE10", 5m), new AppliedCouponDto("SAVE5", 5m)],
            Guid.NewGuid(),
            null,
            Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        dbContext.CouponUsages.Count().Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithUnknownCouponCode_ThrowsCouponNotFoundException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var handler = new CommitCouponUsageCommandHandler(dbContext);
        var command = new CommitCouponUsageCommand(
            [new AppliedCouponDto("UNKNOWN", 5m)],
            Guid.NewGuid(),
            null,
            Guid.NewGuid());

        await Assert.ThrowsAsync<CouponNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
