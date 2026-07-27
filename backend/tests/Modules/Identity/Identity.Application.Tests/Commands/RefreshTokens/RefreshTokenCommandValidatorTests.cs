using Identity.Application.Commands.RefreshTokens;
using Identity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Identity.Application.Tests.Commands.RefreshTokens;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new RefreshTokenCommand("some-refresh-token", ClientPlatform.Web);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyRefreshToken_HasError()
    {
        var command = new RefreshTokenCommand("", ClientPlatform.Web);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RefreshTokenCommand.RefreshToken));
    }
}
