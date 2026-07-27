using FluentAssertions;
using Pricing.Application.Commands.Coupons.CreateCoupon;
using Pricing.Domain.Enums;
using Pricing.Domain.Exceptions;
using Xunit;

namespace Pricing.Application.Tests.Commands.Coupons.CreateCoupon;

public class CreateCouponCommandHandlerTests
{
    private static CreateCouponCommand ValidCommand(string code = "SAVE10") => new(
        code,
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

    [Fact]
    public async Task Handle_WithUniqueCode_CreatesCoupon()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var handler = new CreateCouponCommandHandler(dbContext);

        var couponId = await handler.Handle(ValidCommand(), CancellationToken.None);

        couponId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.Coupons.FindAsync(couponId);
        stored.Should().NotBeNull();
        stored!.Code.Should().Be("SAVE10");
        stored.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithDuplicateCode_ThrowsCouponCodeAlreadyExistsException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        dbContext.Coupons.Add(Pricing.Domain.Entities.Coupon.Create(
            "SAVE10", CouponDiscountType.Percentage, 10, null, null, CouponScopeType.Cart, null,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null, null));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateCouponCommandHandler(dbContext);

        await Assert.ThrowsAsync<CouponCodeAlreadyExistsException>(
            () => handler.Handle(ValidCommand(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInvalidDiscountConfiguration_ThrowsInvalidCouponConfigurationException()
    {
        using var dbContext = TestPricingDbContextFactory.Create();
        var handler = new CreateCouponCommandHandler(dbContext);
        var command = ValidCommand() with { DiscountValue = 0 };

        await Assert.ThrowsAsync<InvalidCouponConfigurationException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
