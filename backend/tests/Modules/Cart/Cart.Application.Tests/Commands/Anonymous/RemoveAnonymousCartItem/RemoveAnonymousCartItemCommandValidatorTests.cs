using Cart.Application.Commands.Anonymous.RemoveAnonymousCartItem;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Commands.Anonymous.RemoveAnonymousCartItem;

public class RemoveAnonymousCartItemCommandValidatorTests
{
    private readonly RemoveAnonymousCartItemCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new RemoveAnonymousCartItemCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyAnonymousId_HasError()
    {
        var result = _validator.Validate(new RemoveAnonymousCartItemCommand(Guid.Empty, Guid.NewGuid()));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveAnonymousCartItemCommand.AnonymousId));
    }

    [Fact]
    public void Validate_WithEmptyCartItemId_HasError()
    {
        var result = _validator.Validate(new RemoveAnonymousCartItemCommand(Guid.NewGuid(), Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveAnonymousCartItemCommand.CartItemId));
    }
}
