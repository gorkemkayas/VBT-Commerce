using FluentAssertions;
using Pricing.Application.Commands.Prices.CreatePrice;
using Pricing.Domain.Enums;
using Xunit;

namespace Pricing.Application.Tests.Commands.Prices.CreatePrice;

public class CreatePriceCommandValidatorTests
{
    private readonly CreatePriceCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreatePriceCommand(Guid.NewGuid(), PriceItemType.Product, 10m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySellableItemId_HasError()
    {
        var command = new CreatePriceCommand(Guid.Empty, PriceItemType.Product, 10m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePriceCommand.SellableItemId));
    }

    [Fact]
    public void Validate_WithInvalidSellableItemType_HasError()
    {
        var command = new CreatePriceCommand(Guid.NewGuid(), (PriceItemType)999, 10m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePriceCommand.SellableItemType));
    }

    [Fact]
    public void Validate_WithZeroAmount_HasError()
    {
        var command = new CreatePriceCommand(Guid.NewGuid(), PriceItemType.Product, 0m);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePriceCommand.Amount));
    }
}
