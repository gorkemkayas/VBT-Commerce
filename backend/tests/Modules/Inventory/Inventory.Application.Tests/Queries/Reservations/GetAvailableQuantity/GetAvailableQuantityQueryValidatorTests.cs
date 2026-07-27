using FluentAssertions;
using Inventory.Application.Queries.Reservations.GetAvailableQuantity;
using Inventory.Domain.Enums;
using Xunit;

namespace Inventory.Application.Tests.Queries.Reservations.GetAvailableQuantity;

public class GetAvailableQuantityQueryValidatorTests
{
    private readonly GetAvailableQuantityQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetAvailableQuantityQuery(Guid.NewGuid(), InventoryItemType.Product);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySellableItemId_HasError()
    {
        var query = new GetAvailableQuantityQuery(Guid.Empty, InventoryItemType.Product);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetAvailableQuantityQuery.SellableItemId));
    }

    [Fact]
    public void Validate_WithInvalidSellableItemType_HasError()
    {
        var query = new GetAvailableQuantityQuery(Guid.NewGuid(), (InventoryItemType)999);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetAvailableQuantityQuery.SellableItemType));
    }
}
