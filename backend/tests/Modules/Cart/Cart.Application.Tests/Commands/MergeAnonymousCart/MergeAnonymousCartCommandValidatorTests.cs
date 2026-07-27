using Cart.Application.Commands.MergeAnonymousCart;
using FluentAssertions;
using Xunit;

namespace Cart.Application.Tests.Commands.MergeAnonymousCart;

public class MergeAnonymousCartCommandValidatorTests
{
    private readonly MergeAnonymousCartCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new MergeAnonymousCartCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyUserId_HasError()
    {
        var result = _validator.Validate(new MergeAnonymousCartCommand(Guid.Empty, Guid.NewGuid()));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(MergeAnonymousCartCommand.UserId));
    }

    [Fact]
    public void Validate_WithEmptyAnonymousId_HasError()
    {
        var result = _validator.Validate(new MergeAnonymousCartCommand(Guid.NewGuid(), Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(MergeAnonymousCartCommand.AnonymousId));
    }
}
