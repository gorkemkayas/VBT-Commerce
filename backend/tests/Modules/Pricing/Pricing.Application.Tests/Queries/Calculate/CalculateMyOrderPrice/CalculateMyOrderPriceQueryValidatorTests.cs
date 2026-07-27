using FluentAssertions;
using Pricing.Application.Queries.Calculate.CalculateMyOrderPrice;
using Pricing.Contracts;
using Pricing.Domain.Enums;
using Xunit;

namespace Pricing.Application.Tests.Queries.Calculate.CalculateMyOrderPrice;

public class CalculateMyOrderPriceQueryValidatorTests
{
    private readonly CalculateMyOrderPriceQueryValidator _validator = new();

    private static CalculateMyOrderPriceQuery ValidQuery() => new(
        [new PriceCalculationItem(Guid.NewGuid(), PriceItemType.Product, 1)],
        ["SAVE10"]);

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(ValidQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyItems_HasError()
    {
        var query = ValidQuery() with { Items = [] };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CalculateMyOrderPriceQuery.Items));
    }

    [Fact]
    public void Validate_WithInvalidItemType_HasError()
    {
        var query = ValidQuery() with { Items = [new PriceCalculationItem(Guid.NewGuid(), (PriceItemType)999, 1)] };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithNegativeQuantity_HasError()
    {
        var query = ValidQuery() with { Items = [new PriceCalculationItem(Guid.NewGuid(), PriceItemType.Product, -1)] };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithEmptyCouponCode_HasError()
    {
        var query = ValidQuery() with { CouponCodes = ["  "] };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }
}
