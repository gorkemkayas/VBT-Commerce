using FluentAssertions;
using Shipping.Application.Queries.Shipments.GetShipmentByOrderId;
using Xunit;

namespace Shipping.Application.Tests.Queries.Shipments;

public class GetShipmentByOrderIdQueryValidatorTests
{
    private readonly GetShipmentByOrderIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetShipmentByOrderIdQuery(Guid.NewGuid());

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var query = new GetShipmentByOrderIdQuery(Guid.Empty);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetShipmentByOrderIdQuery.OrderId));
    }
}
