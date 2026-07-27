using Cart.Application.Commands.Anonymous.UpdateAnonymousCartItemQuantity;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Commands.Anonymous.UpdateAnonymousCartItemQuantity;

public class UpdateAnonymousCartItemQuantityCommandValidatorTests
{
    private readonly UpdateAnonymousCartItemQuantityCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new UpdateAnonymousCartItemQuantityCommand(Guid.NewGuid(), Guid.NewGuid(), 1));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyAnonymousId_HasError()
    {
        var result = _validator.Validate(new UpdateAnonymousCartItemQuantityCommand(Guid.Empty, Guid.NewGuid(), 1));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateAnonymousCartItemQuantityCommand.AnonymousId));
    }

    [Fact]
    public void Validate_WithEmptyCartItemId_HasError()
    {
        var result = _validator.Validate(new UpdateAnonymousCartItemQuantityCommand(Guid.NewGuid(), Guid.Empty, 1));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateAnonymousCartItemQuantityCommand.CartItemId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveQuantity_HasError(int quantity)
    {
        var result = _validator.Validate(new UpdateAnonymousCartItemQuantityCommand(Guid.NewGuid(), Guid.NewGuid(), quantity));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateAnonymousCartItemQuantityCommand.Quantity));
    }
}
