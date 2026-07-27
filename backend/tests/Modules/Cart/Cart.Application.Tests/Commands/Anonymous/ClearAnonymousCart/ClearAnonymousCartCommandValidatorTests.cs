using Cart.Application.Commands.Anonymous.ClearAnonymousCart;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Commands.Anonymous.ClearAnonymousCart;

public class ClearAnonymousCartCommandValidatorTests
{
    private readonly ClearAnonymousCartCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new ClearAnonymousCartCommand(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyAnonymousId_HasError()
    {
        var result = _validator.Validate(new ClearAnonymousCartCommand(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ClearAnonymousCartCommand.AnonymousId));
    }
}
