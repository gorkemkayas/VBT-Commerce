using Identity.Application.Commands.ForgotPassword;
using FluentAssertions;
using Xunit;

namespace Identity.Application.Tests.Commands.ForgotPassword;

public class ForgotPasswordCommandValidatorTests
{
    private readonly ForgotPasswordCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new ForgotPasswordCommand("jane@example.com");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_HasError(string email)
    {
        var command = new ForgotPasswordCommand(email);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ForgotPasswordCommand.Email));
    }
}
