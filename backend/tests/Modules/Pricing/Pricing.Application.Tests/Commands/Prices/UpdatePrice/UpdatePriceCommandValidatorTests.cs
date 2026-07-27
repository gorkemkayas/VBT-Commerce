using FluentAssertions;
using Pricing.Application.Commands.Prices.UpdatePrice;
using Xunit;

namespace Pricing.Application.Tests.Commands.Prices.UpdatePrice;

public class UpdatePriceCommandValidatorTests
{
    private readonly UpdatePriceCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new UpdatePriceCommand(Guid.NewGuid(), 10m));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyPriceId_HasError()
    {
        var result = _validator.Validate(new UpdatePriceCommand(Guid.Empty, 10m));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdatePriceCommand.PriceId));
    }

    [Fact]
    public void Validate_WithZeroAmount_HasError()
    {
        var result = _validator.Validate(new UpdatePriceCommand(Guid.NewGuid(), 0m));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdatePriceCommand.Amount));
    }
}
