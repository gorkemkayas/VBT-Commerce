using Identity.Application.Commands.ResetPassword;
using FluentAssertions;
using Xunit;

namespace Identity.Application.Tests.Commands.ResetPassword;

public class ResetPasswordCommandValidatorTests
{
    private readonly ResetPasswordCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new ResetPasswordCommand("some-token", "NewPassword123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyToken_HasError()
    {
        var command = new ResetPasswordCommand("", "NewPassword123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ResetPasswordCommand.Token));
    }

    [Theory]
    [InlineData("")]
    [InlineData("short1")]
    public void Validate_WithInvalidNewPassword_HasError(string newPassword)
    {
        var command = new ResetPasswordCommand("some-token", newPassword);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ResetPasswordCommand.NewPassword));
    }
}
