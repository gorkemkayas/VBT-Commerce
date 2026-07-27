using Customer.Application.Commands.GuestCustomers.CreateGuestCustomer;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Commands.GuestCustomers;

public class CreateGuestCustomerCommandValidatorTests
{
    private readonly CreateGuestCustomerCommandValidator _validator = new();

    private static CreateGuestCustomerCommand ValidCommand() =>
        new("Jane", "Doe", "jane@example.com", "5551234567");

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyFirstName_HasError(string firstName)
    {
        var command = ValidCommand() with { FirstName = firstName };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGuestCustomerCommand.FirstName));
    }

    [Fact]
    public void Validate_WithFirstNameTooLong_HasError()
    {
        var command = ValidCommand() with { FirstName = new string('a', 101) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGuestCustomerCommand.FirstName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyLastName_HasError(string lastName)
    {
        var command = ValidCommand() with { LastName = lastName };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGuestCustomerCommand.LastName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_HasError(string email)
    {
        var command = ValidCommand() with { Email = email };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGuestCustomerCommand.Email));
    }

    [Fact]
    public void Validate_WithEmptyPhoneNumber_HasError()
    {
        var command = ValidCommand() with { PhoneNumber = "" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGuestCustomerCommand.PhoneNumber));
    }
}
