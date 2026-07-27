using FluentAssertions;
using Order.Application.Commands.Me.CancelMyOrder;
using Xunit;

namespace Order.Application.Tests.Commands.Me.CancelMyOrder;

public class CancelMyOrderCommandValidatorTests
{
    private readonly CancelMyOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CancelMyOrderCommand(Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var command = new CancelMyOrderCommand(Guid.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CancelMyOrderCommand.OrderId));
    }
}
