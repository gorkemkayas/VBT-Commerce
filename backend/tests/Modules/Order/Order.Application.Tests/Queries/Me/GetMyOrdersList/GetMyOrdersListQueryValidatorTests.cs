using FluentAssertions;
using Order.Application.Queries.Me.GetMyOrdersList;
using Xunit;

namespace Order.Application.Tests.Queries.Me.GetMyOrdersList;

public class GetMyOrdersListQueryValidatorTests
{
    private readonly GetMyOrdersListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithDefaultValues_HasNoErrors()
    {
        var result = _validator.Validate(new GetMyOrdersListQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPageNumberLessThanOne_HasError()
    {
        var result = _validator.Validate(new GetMyOrdersListQuery(PageNumber: 0));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetMyOrdersListQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutOfRange_HasError(int pageSize)
    {
        var result = _validator.Validate(new GetMyOrdersListQuery(PageSize: pageSize));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetMyOrdersListQuery.PageSize));
    }
}
