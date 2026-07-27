using FluentAssertions;
using Pricing.Application.Queries.Prices.GetPrice;
using Pricing.Domain.Enums;
using Xunit;

namespace Pricing.Application.Tests.Queries.Prices.GetPrice;

public class GetPriceQueryValidatorTests
{
    private readonly GetPriceQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetPriceQuery(Guid.NewGuid(), PriceItemType.Product));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySellableItemId_HasError()
    {
        var result = _validator.Validate(new GetPriceQuery(Guid.Empty, PriceItemType.Product));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPriceQuery.SellableItemId));
    }

    [Fact]
    public void Validate_WithInvalidSellableItemType_HasError()
    {
        var result = _validator.Validate(new GetPriceQuery(Guid.NewGuid(), (PriceItemType)999));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPriceQuery.SellableItemType));
    }
}
