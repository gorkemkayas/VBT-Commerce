using FluentAssertions;
using Inventory.Application.Queries.StockItems.GetStockItemBySellableItem;
using Inventory.Domain.Enums;
using Xunit;

namespace Inventory.Application.Tests.Queries.StockItems.GetStockItemBySellableItem;

public class GetStockItemBySellableItemQueryValidatorTests
{
    private readonly GetStockItemBySellableItemQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetStockItemBySellableItemQuery(Guid.NewGuid(), InventoryItemType.Product);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySellableItemId_HasError()
    {
        var query = new GetStockItemBySellableItemQuery(Guid.Empty, InventoryItemType.Product);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetStockItemBySellableItemQuery.SellableItemId));
    }

    [Fact]
    public void Validate_WithInvalidSellableItemType_HasError()
    {
        var query = new GetStockItemBySellableItemQuery(Guid.NewGuid(), (InventoryItemType)999);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetStockItemBySellableItemQuery.SellableItemType));
    }
}
