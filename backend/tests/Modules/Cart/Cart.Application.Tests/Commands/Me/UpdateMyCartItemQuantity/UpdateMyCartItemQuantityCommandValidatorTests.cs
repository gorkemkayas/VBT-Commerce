using Cart.Application.Commands.Me.UpdateMyCartItemQuantity;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Commands.Me.UpdateMyCartItemQuantity;

public class UpdateMyCartItemQuantityCommandValidatorTests
{
    private readonly UpdateMyCartItemQuantityCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new UpdateMyCartItemQuantityCommand(Guid.NewGuid(), 1));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCartItemId_HasError()
    {
        var result = _validator.Validate(new UpdateMyCartItemQuantityCommand(Guid.Empty, 1));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCartItemQuantityCommand.CartItemId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveQuantity_HasError(int quantity)
    {
        var result = _validator.Validate(new UpdateMyCartItemQuantityCommand(Guid.NewGuid(), quantity));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCartItemQuantityCommand.Quantity));
    }
}
