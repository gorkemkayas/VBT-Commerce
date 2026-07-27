using FluentAssertions;
using Shipping.Application.Commands.ShippingCompanies.DeactivateShippingCompany;
using Xunit;

namespace Shipping.Application.Tests.Commands.ShippingCompanies;

public class DeactivateShippingCompanyCommandValidatorTests
{
    private readonly DeactivateShippingCompanyCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new DeactivateShippingCompanyCommand(Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyShippingCompanyId_HasError()
    {
        var command = new DeactivateShippingCompanyCommand(Guid.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DeactivateShippingCompanyCommand.ShippingCompanyId));
    }
}
