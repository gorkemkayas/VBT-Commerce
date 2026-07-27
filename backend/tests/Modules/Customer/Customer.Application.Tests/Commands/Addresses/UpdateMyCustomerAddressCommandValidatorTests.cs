using Customer.Application.Commands.Addresses.UpdateMyCustomerAddress;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Commands.Addresses;

public class UpdateMyCustomerAddressCommandValidatorTests
{
    private readonly UpdateMyCustomerAddressCommandValidator _validator = new();

    private static UpdateMyCustomerAddressCommand ValidCommand(
        bool isShipping = true, bool isBilling = true) =>
        new(Guid.NewGuid(), "Home", "John Doe", "5551234567", "Turkey", "Istanbul", "Kadikoy", "34000",
            "Some street 1", null, false, isShipping, isBilling);

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyAddressId_HasError()
    {
        var command = ValidCommand() with { AddressId = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.AddressId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyLabel_HasError(string label)
    {
        var command = ValidCommand() with { Label = label };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.Label));
    }

    [Fact]
    public void Validate_WithLabelTooLong_HasError()
    {
        var command = ValidCommand() with { Label = new string('a', 51) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.Label));
    }

    [Fact]
    public void Validate_WithEmptyRecipientName_HasError()
    {
        var command = ValidCommand() with { RecipientName = "" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.RecipientName));
    }

    [Fact]
    public void Validate_WithEmptyPhoneNumber_HasError()
    {
        var command = ValidCommand() with { PhoneNumber = "" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.PhoneNumber));
    }

    [Fact]
    public void Validate_WithEmptyCountry_HasError()
    {
        var command = ValidCommand() with { Country = "" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.Country));
    }

    [Fact]
    public void Validate_WithEmptyCity_HasError()
    {
        var command = ValidCommand() with { City = "" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.City));
    }

    [Fact]
    public void Validate_WithEmptyDistrict_HasError()
    {
        var command = ValidCommand() with { District = "" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.District));
    }

    [Fact]
    public void Validate_WithEmptyPostalCode_HasError()
    {
        var command = ValidCommand() with { PostalCode = "" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.PostalCode));
    }

    [Fact]
    public void Validate_WithEmptyAddressLine1_HasError()
    {
        var command = ValidCommand() with { AddressLine1 = "" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.AddressLine1));
    }

    [Fact]
    public void Validate_WithAddressLine2TooLong_HasError()
    {
        var command = ValidCommand() with { AddressLine2 = new string('a', 301) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyCustomerAddressCommand.AddressLine2));
    }

    [Fact]
    public void Validate_WithNeitherShippingNorBilling_HasError()
    {
        var command = ValidCommand(isShipping: false, isBilling: false);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
