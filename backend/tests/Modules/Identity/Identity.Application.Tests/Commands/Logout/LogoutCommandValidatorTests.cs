using Identity.Application.Commands.Logout;
using FluentAssertions;
using Xunit;

namespace Identity.Application.Tests.Commands.Logout;

public class LogoutCommandValidatorTests
{
    private readonly LogoutCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new LogoutCommand("some-refresh-token");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyRefreshToken_HasError()
    {
        var command = new LogoutCommand("");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LogoutCommand.RefreshToken));
    }
}
