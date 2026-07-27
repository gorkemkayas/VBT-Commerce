using FluentAssertions;
using Shipping.Application.Queries.Shipments.GetShipmentById;
using Xunit;

namespace Shipping.Application.Tests.Queries.Shipments;

public class GetShipmentByIdQueryValidatorTests
{
    private readonly GetShipmentByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetShipmentByIdQuery(Guid.NewGuid());

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyShipmentId_HasError()
    {
        var query = new GetShipmentByIdQuery(Guid.Empty);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetShipmentByIdQuery.ShipmentId));
    }
}
