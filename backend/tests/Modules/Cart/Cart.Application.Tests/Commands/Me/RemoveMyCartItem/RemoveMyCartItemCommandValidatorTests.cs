using Cart.Application.Commands.Me.RemoveMyCartItem;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Commands.Me.RemoveMyCartItem;

public class RemoveMyCartItemCommandValidatorTests
{
    private readonly RemoveMyCartItemCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new RemoveMyCartItemCommand(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCartItemId_HasError()
    {
        var result = _validator.Validate(new RemoveMyCartItemCommand(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveMyCartItemCommand.CartItemId));
    }
}
