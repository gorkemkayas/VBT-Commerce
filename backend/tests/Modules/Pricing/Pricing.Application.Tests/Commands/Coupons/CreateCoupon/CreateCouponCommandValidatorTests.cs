using FluentAssertions;
using Pricing.Application.Commands.Coupons.CreateCoupon;
using Pricing.Domain.Enums;
using Xunit;

namespace Pricing.Application.Tests.Commands.Coupons.CreateCoupon;

public class CreateCouponCommandValidatorTests
{
    private readonly CreateCouponCommandValidator _validator = new();

    private static CreateCouponCommand ValidCommand() => new(
        "SAVE10",
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

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyCode_HasError(string code)
    {
        var command = ValidCommand() with { Code = code };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCouponCommand.Code));
    }

    [Fact]
    public void Validate_WithCodeTooLong_HasError()
    {
        var command = ValidCommand() with { Code = new string('a', 51) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCouponCommand.Code));
    }

    [Fact]
    public void Validate_WithZeroDiscountValue_HasError()
    {
        var command = ValidCommand() with { DiscountValue = 0 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCouponCommand.DiscountValue));
    }

    [Fact]
    public void Validate_WithZeroMaxDiscountAmount_HasError()
    {
        var command = ValidCommand() with { MaxDiscountAmount = 0 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCouponCommand.MaxDiscountAmount));
    }

    [Fact]
    public void Validate_WithZeroMinCartAmount_HasError()
    {
        var command = ValidCommand() with { MinCartAmount = 0 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCouponCommand.MinCartAmount));
    }

    [Fact]
    public void Validate_WithEndDateBeforeStartDate_HasError()
    {
        var command = ValidCommand() with { EndDate = DateTime.UtcNow.AddDays(-1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCouponCommand.EndDate));
    }

    [Fact]
    public void Validate_WithZeroTotalUsageLimit_HasError()
    {
        var command = ValidCommand() with { TotalUsageLimit = 0 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCouponCommand.TotalUsageLimit));
    }

    [Fact]
    public void Validate_WithZeroPerUserUsageLimit_HasError()
    {
        var command = ValidCommand() with { PerUserUsageLimit = 0 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCouponCommand.PerUserUsageLimit));
    }

    [Fact]
    public void Validate_WithInvalidDiscountType_HasError()
    {
        var command = ValidCommand() with { DiscountType = (CouponDiscountType)999 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCouponCommand.DiscountType));
    }

    [Fact]
    public void Validate_WithInvalidScopeType_HasError()
    {
        var command = ValidCommand() with { ScopeType = (CouponScopeType)999 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCouponCommand.ScopeType));
    }
}
