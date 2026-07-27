using FluentAssertions;
using Pricing.Application.Commands.Coupons.DeactivateCoupon;
using Xunit;

namespace Pricing.Application.Tests.Commands.Coupons.DeactivateCoupon;

public class DeactivateCouponCommandValidatorTests
{
    private readonly DeactivateCouponCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new DeactivateCouponCommand(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCouponId_HasError()
    {
        var result = _validator.Validate(new DeactivateCouponCommand(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DeactivateCouponCommand.CouponId));
    }
}
