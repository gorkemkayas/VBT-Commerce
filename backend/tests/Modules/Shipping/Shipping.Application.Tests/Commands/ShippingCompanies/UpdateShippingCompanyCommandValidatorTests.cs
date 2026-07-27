using FluentAssertions;
using Shipping.Application.Commands.ShippingCompanies.UpdateShippingCompany;
using Xunit;

namespace Shipping.Application.Tests.Commands.ShippingCompanies;

public class UpdateShippingCompanyCommandValidatorTests
{
    private readonly UpdateShippingCompanyCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new UpdateShippingCompanyCommand(Guid.NewGuid(), "Aras Kargo", 25m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyShippingCompanyId_HasError()
    {
        var command = new UpdateShippingCompanyCommand(Guid.Empty, "Aras Kargo", 25m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateShippingCompanyCommand.ShippingCompanyId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyName_HasError(string name)
    {
        var command = new UpdateShippingCompanyCommand(Guid.NewGuid(), name, 25m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateShippingCompanyCommand.Name));
    }

    [Fact]
    public void Validate_WithNameTooLong_HasError()
    {
        var command = new UpdateShippingCompanyCommand(Guid.NewGuid(), new string('a', 201), 25m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateShippingCompanyCommand.Name));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_WithNonPositiveFee_HasError(decimal fee)
    {
        var command = new UpdateShippingCompanyCommand(Guid.NewGuid(), "Aras Kargo", fee);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateShippingCompanyCommand.Fee));
    }
}
