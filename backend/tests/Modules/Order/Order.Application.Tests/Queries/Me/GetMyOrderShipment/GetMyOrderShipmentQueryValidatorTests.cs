using FluentAssertions;
using Order.Application.Queries.Me.GetMyOrderShipment;
using Xunit;

namespace Order.Application.Tests.Queries.Me.GetMyOrderShipment;

public class GetMyOrderShipmentQueryValidatorTests
{
    private readonly GetMyOrderShipmentQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetMyOrderShipmentQuery(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var result = _validator.Validate(new GetMyOrderShipmentQuery(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetMyOrderShipmentQuery.OrderId));
    }
}
