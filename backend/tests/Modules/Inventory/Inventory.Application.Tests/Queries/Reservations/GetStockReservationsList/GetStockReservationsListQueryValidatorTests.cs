using FluentAssertions;
using Inventory.Application.Queries.Reservations.GetStockReservationsList;
using Xunit;

namespace Inventory.Application.Tests.Queries.Reservations.GetStockReservationsList;

public class GetStockReservationsListQueryValidatorTests
{
    private readonly GetStockReservationsListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithDefaultQuery_HasNoErrors()
    {
        var query = new GetStockReservationsListQuery();

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPageNumberLessThanOne_HasError()
    {
        var query = new GetStockReservationsListQuery(PageNumber: 0);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetStockReservationsListQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutsideRange_HasError(int pageSize)
    {
        var query = new GetStockReservationsListQuery(PageSize: pageSize);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetStockReservationsListQuery.PageSize));
    }
}
