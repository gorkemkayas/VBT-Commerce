using FluentAssertions;
using Shipping.Application.Queries.Shipments.GetShipmentsList;
using Shipping.Domain.Enums;
using Xunit;

namespace Shipping.Application.Tests.Queries.Shipments;

public class GetShipmentsListQueryValidatorTests
{
    private readonly GetShipmentsListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetShipmentsListQuery(ShipmentStatus.Pending, 1, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNullStatus_HasNoErrors()
    {
        var query = new GetShipmentsListQuery(null, 1, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithInvalidStatus_HasError()
    {
        var query = new GetShipmentsListQuery((ShipmentStatus)999, 1, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetShipmentsListQuery.Status));
    }

    [Fact]
    public void Validate_WithPageNumberLessThanOne_HasError()
    {
        var query = new GetShipmentsListQuery(null, 0, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetShipmentsListQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutOfRange_HasError(int pageSize)
    {
        var query = new GetShipmentsListQuery(null, 1, pageSize);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetShipmentsListQuery.PageSize));
    }
}
