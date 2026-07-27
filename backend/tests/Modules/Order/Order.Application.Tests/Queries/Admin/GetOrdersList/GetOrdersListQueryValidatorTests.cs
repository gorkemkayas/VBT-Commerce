using FluentAssertions;
using Order.Application.Queries.Admin.GetOrdersList;
using Order.Domain.Enums;
using Xunit;

namespace Order.Application.Tests.Queries.Admin.GetOrdersList;

public class GetOrdersListQueryValidatorTests
{
    private readonly GetOrdersListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithDefaultValues_HasNoErrors()
    {
        var result = _validator.Validate(new GetOrdersListQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidStatus_HasNoErrors()
    {
        var result = _validator.Validate(new GetOrdersListQuery(OrderStatus.Confirmed));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithInvalidStatus_HasError()
    {
        var command = new GetOrdersListQuery((OrderStatus)999);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetOrdersListQuery.Status));
    }

    [Fact]
    public void Validate_WithPageNumberLessThanOne_HasError()
    {
        var command = new GetOrdersListQuery(PageNumber: 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetOrdersListQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutOfRange_HasError(int pageSize)
    {
        var command = new GetOrdersListQuery(PageSize: pageSize);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetOrdersListQuery.PageSize));
    }
}
