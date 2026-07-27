using FluentAssertions;
using Order.Application.Commands.Admin.CancelOrder;
using Xunit;

namespace Order.Application.Tests.Commands.Admin.CancelOrder;

public class CancelOrderCommandValidatorTests
{
    private readonly CancelOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CancelOrderCommand(Guid.NewGuid(), "Some reason");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNullReason_HasNoErrors()
    {
        var command = new CancelOrderCommand(Guid.NewGuid(), null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var command = new CancelOrderCommand(Guid.Empty, "reason");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CancelOrderCommand.OrderId));
    }

    [Fact]
    public void Validate_WithReasonTooLong_HasError()
    {
        var command = new CancelOrderCommand(Guid.NewGuid(), new string('a', 501));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CancelOrderCommand.Reason));
    }
}
