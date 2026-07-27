using Identity.Application.Commands.Register;
using Identity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Identity.Application.Tests.Commands.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new RegisterCommand("jane@example.com", "Password123", "Jane", "Doe", ClientPlatform.Web);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_HasError(string email)
    {
        var command = new RegisterCommand(email, "Password123", "Jane", "Doe", ClientPlatform.Web);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("short1")]
    public void Validate_WithInvalidPassword_HasError(string password)
    {
        var command = new RegisterCommand("jane@example.com", password, "Jane", "Doe", ClientPlatform.Web);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Password));
    }

    [Fact]
    public void Validate_WithEmptyFirstName_HasError()
    {
        var command = new RegisterCommand("jane@example.com", "Password123", "", "Doe", ClientPlatform.Web);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.FirstName));
    }

    [Fact]
    public void Validate_WithFirstNameTooLong_HasError()
    {
        var command = new RegisterCommand("jane@example.com", "Password123", new string('a', 101), "Doe", ClientPlatform.Web);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.FirstName));
    }

    [Fact]
    public void Validate_WithEmptyLastName_HasError()
    {
        var command = new RegisterCommand("jane@example.com", "Password123", "Jane", "", ClientPlatform.Web);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.LastName));
    }

    [Fact]
    public void Validate_WithLastNameTooLong_HasError()
    {
        var command = new RegisterCommand("jane@example.com", "Password123", "Jane", new string('a', 101), ClientPlatform.Web);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.LastName));
    }
}
