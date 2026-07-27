using FluentAssertions;
using Inventory.Application.Queries.StockItems.GetStockItemsList;
using Xunit;

namespace Inventory.Application.Tests.Queries.StockItems.GetStockItemsList;

public class GetStockItemsListQueryValidatorTests
{
    private readonly GetStockItemsListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithDefaultQuery_HasNoErrors()
    {
        var query = new GetStockItemsListQuery();

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPageNumberLessThanOne_HasError()
    {
        var query = new GetStockItemsListQuery(PageNumber: 0);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetStockItemsListQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutsideRange_HasError(int pageSize)
    {
        var query = new GetStockItemsListQuery(PageSize: pageSize);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetStockItemsListQuery.PageSize));
    }
}
