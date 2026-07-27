using FluentAssertions;
using Pricing.Application.Commands.CouponUsage.CommitCouponUsage;
using Pricing.Contracts;
using Xunit;

namespace Pricing.Application.Tests.Commands.CouponUsage.CommitCouponUsage;

public class CommitCouponUsageCommandValidatorTests
{
    private readonly CommitCouponUsageCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CommitCouponUsageCommand(
            [new AppliedCouponDto("SAVE10", 5m)],
            Guid.NewGuid(),
            null,
            Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var command = new CommitCouponUsageCommand(
            [new AppliedCouponDto("SAVE10", 5m)],
            Guid.NewGuid(),
            null,
            Guid.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CommitCouponUsageCommand.OrderId));
    }

    [Fact]
    public void Validate_WithBothCustomerIdAndGuestCustomerId_HasError()
    {
        var command = new CommitCouponUsageCommand(
            [new AppliedCouponDto("SAVE10", 5m)],
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithNeitherCustomerIdNorGuestCustomerId_HasError()
    {
        var command = new CommitCouponUsageCommand(
            [new AppliedCouponDto("SAVE10", 5m)],
            null,
            null,
            Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
