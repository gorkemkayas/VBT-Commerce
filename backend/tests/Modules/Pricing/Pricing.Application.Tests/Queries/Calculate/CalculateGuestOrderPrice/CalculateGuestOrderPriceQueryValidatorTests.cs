using FluentAssertions;
using Pricing.Application.Queries.Calculate.CalculateGuestOrderPrice;
using Pricing.Contracts;
using Pricing.Domain.Enums;
using Xunit;

namespace Pricing.Application.Tests.Queries.Calculate.CalculateGuestOrderPrice;

public class CalculateGuestOrderPriceQueryValidatorTests
{
    private readonly CalculateGuestOrderPriceQueryValidator _validator = new();

    private static CalculateGuestOrderPriceQuery ValidQuery() => new(
        Guid.NewGuid(),
        [new PriceCalculationItem(Guid.NewGuid(), PriceItemType.Product, 1)],
        ["SAVE10"]);

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(ValidQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyGuestCustomerId_HasError()
    {
        var query = ValidQuery() with { GuestCustomerId = Guid.Empty };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CalculateGuestOrderPriceQuery.GuestCustomerId));
    }

    [Fact]
    public void Validate_WithEmptyItems_HasError()
    {
        var query = ValidQuery() with { Items = [] };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CalculateGuestOrderPriceQuery.Items));
    }

    [Fact]
    public void Validate_WithZeroQuantityItem_HasError()
    {
        var query = ValidQuery() with { Items = [new PriceCalculationItem(Guid.NewGuid(), PriceItemType.Product, 0)] };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithEmptyItemSellableItemId_HasError()
    {
        var query = ValidQuery() with { Items = [new PriceCalculationItem(Guid.Empty, PriceItemType.Product, 1)] };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithEmptyCouponCode_HasError()
    {
        var query = ValidQuery() with { CouponCodes = [""] };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }
}
