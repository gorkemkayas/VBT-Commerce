using Customer.Application.Commands.Profile.CreateMyCustomerProfile;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Commands.Profile;

public class CreateMyCustomerProfileCommandValidatorTests
{
    private readonly CreateMyCustomerProfileCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateMyCustomerProfileCommand("5551234567", new DateOnly(1990, 1, 1));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNullPhoneNumberAndDateOfBirth_HasNoErrors()
    {
        var command = new CreateMyCustomerProfileCommand(null, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPhoneNumberTooLong_HasError()
    {
        var command = new CreateMyCustomerProfileCommand(new string('1', 31), null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateMyCustomerProfileCommand.PhoneNumber));
    }

    [Fact]
    public void Validate_WithFutureDateOfBirth_HasError()
    {
        var command = new CreateMyCustomerProfileCommand(null, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateMyCustomerProfileCommand.DateOfBirth));
    }
}
