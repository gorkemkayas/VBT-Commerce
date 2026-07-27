using FluentAssertions;
using Shipping.Application.Commands.ShippingCompanies.CreateShippingCompany;
using Xunit;

namespace Shipping.Application.Tests.Commands.ShippingCompanies;

public class CreateShippingCompanyCommandValidatorTests
{
    private readonly CreateShippingCompanyCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateShippingCompanyCommand("Aras Kargo", 25m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyName_HasError(string name)
    {
        var command = new CreateShippingCompanyCommand(name, 25m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateShippingCompanyCommand.Name));
    }

    [Fact]
    public void Validate_WithNameTooLong_HasError()
    {
        var command = new CreateShippingCompanyCommand(new string('a', 201), 25m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateShippingCompanyCommand.Name));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_WithNonPositiveFee_HasError(decimal fee)
    {
        var command = new CreateShippingCompanyCommand("Aras Kargo", fee);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateShippingCompanyCommand.Fee));
    }
}
