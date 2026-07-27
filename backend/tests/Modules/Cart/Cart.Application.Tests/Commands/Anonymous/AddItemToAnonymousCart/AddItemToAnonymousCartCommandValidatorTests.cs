using Cart.Application.Commands.Anonymous.AddItemToAnonymousCart;
using Cart.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Commands.Anonymous.AddItemToAnonymousCart;

public class AddItemToAnonymousCartCommandValidatorTests
{
    private readonly AddItemToAnonymousCartCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new AddItemToAnonymousCartCommand(Guid.NewGuid(), Guid.NewGuid(), CartItemType.Product, 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyAnonymousId_HasError()
    {
        var command = new AddItemToAnonymousCartCommand(Guid.Empty, Guid.NewGuid(), CartItemType.Product, 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddItemToAnonymousCartCommand.AnonymousId));
    }

    [Fact]
    public void Validate_WithEmptySellableItemId_HasError()
    {
        var command = new AddItemToAnonymousCartCommand(Guid.NewGuid(), Guid.Empty, CartItemType.Product, 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddItemToAnonymousCartCommand.SellableItemId));
    }

    [Fact]
    public void Validate_WithInvalidSellableItemType_HasError()
    {
        var command = new AddItemToAnonymousCartCommand(Guid.NewGuid(), Guid.NewGuid(), (CartItemType)99, 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddItemToAnonymousCartCommand.SellableItemType));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveQuantity_HasError(int quantity)
    {
        var command = new AddItemToAnonymousCartCommand(Guid.NewGuid(), Guid.NewGuid(), CartItemType.Product, quantity);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddItemToAnonymousCartCommand.Quantity));
    }
}
