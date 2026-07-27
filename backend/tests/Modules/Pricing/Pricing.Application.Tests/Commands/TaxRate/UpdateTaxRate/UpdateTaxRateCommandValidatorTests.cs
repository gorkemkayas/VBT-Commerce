using FluentAssertions;
using Pricing.Application.Commands.TaxRate.UpdateTaxRate;
using Xunit;

namespace Pricing.Application.Tests.Commands.TaxRate.UpdateTaxRate;

public class UpdateTaxRateCommandValidatorTests
{
    private readonly UpdateTaxRateCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(18)]
    [InlineData(100)]
    public void Validate_WithRateInRange_HasNoErrors(decimal rate)
    {
        var result = _validator.Validate(new UpdateTaxRateCommand(rate));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_WithRateOutOfRange_HasError(decimal rate)
    {
        var result = _validator.Validate(new UpdateTaxRateCommand(rate));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateTaxRateCommand.Rate));
    }
}
