using Cart.Application.Commands.Me.AddItemToMyCart;
using Cart.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Commands.Me.AddItemToMyCart;

public class AddItemToMyCartCommandValidatorTests
{
    private readonly AddItemToMyCartCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new AddItemToMyCartCommand(Guid.NewGuid(), CartItemType.Product, 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySellableItemId_HasError()
    {
        var command = new AddItemToMyCartCommand(Guid.Empty, CartItemType.Product, 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddItemToMyCartCommand.SellableItemId));
    }

    [Fact]
    public void Validate_WithInvalidSellableItemType_HasError()
    {
        var command = new AddItemToMyCartCommand(Guid.NewGuid(), (CartItemType)99, 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddItemToMyCartCommand.SellableItemType));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveQuantity_HasError(int quantity)
    {
        var command = new AddItemToMyCartCommand(Guid.NewGuid(), CartItemType.Product, quantity);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddItemToMyCartCommand.Quantity));
    }
}
