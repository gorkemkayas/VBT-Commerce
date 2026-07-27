using FluentAssertions;
using Pricing.Application.Commands.Coupons.UpdateCoupon;
using Pricing.Domain.Enums;
using Xunit;

namespace Pricing.Application.Tests.Commands.Coupons.UpdateCoupon;

public class UpdateCouponCommandValidatorTests
{
    private readonly UpdateCouponCommandValidator _validator = new();

    private static UpdateCouponCommand ValidCommand() => new(
        Guid.NewGuid(),
        CouponDiscountType.Percentage,
        10,
        50,
        100,
        CouponScopeType.Cart,
        null,
        DateTime.UtcNow,
        DateTime.UtcNow.AddDays(30),
        100,
        1);

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCouponId_HasError()
    {
        var command = ValidCommand() with { CouponId = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateCouponCommand.CouponId));
    }

    [Fact]
    public void Validate_WithZeroDiscountValue_HasError()
    {
        var command = ValidCommand() with { DiscountValue = 0 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateCouponCommand.DiscountValue));
    }

    [Fact]
    public void Validate_WithEndDateBeforeStartDate_HasError()
    {
        var command = ValidCommand() with { EndDate = DateTime.UtcNow.AddDays(-1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateCouponCommand.EndDate));
    }

    [Fact]
    public void Validate_WithInvalidDiscountType_HasError()
    {
        var command = ValidCommand() with { DiscountType = (CouponDiscountType)999 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateCouponCommand.DiscountType));
    }

    [Fact]
    public void Validate_WithZeroTotalUsageLimit_HasError()
    {
        var command = ValidCommand() with { TotalUsageLimit = 0 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateCouponCommand.TotalUsageLimit));
    }
}
