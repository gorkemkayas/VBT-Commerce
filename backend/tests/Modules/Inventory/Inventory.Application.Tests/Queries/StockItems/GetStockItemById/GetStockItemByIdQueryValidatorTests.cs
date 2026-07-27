using FluentAssertions;
using Inventory.Application.Queries.StockItems.GetStockItemById;
using Xunit;

namespace Inventory.Application.Tests.Queries.StockItems.GetStockItemById;

public class GetStockItemByIdQueryValidatorTests
{
    private readonly GetStockItemByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetStockItemByIdQuery(Guid.NewGuid());

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyStockItemId_HasError()
    {
        var query = new GetStockItemByIdQuery(Guid.Empty);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetStockItemByIdQuery.StockItemId));
    }
}
