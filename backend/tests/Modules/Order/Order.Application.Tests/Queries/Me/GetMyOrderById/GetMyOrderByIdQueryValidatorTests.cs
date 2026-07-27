using FluentAssertions;
using Order.Application.Queries.Me.GetMyOrderById;
using Xunit;

namespace Order.Application.Tests.Queries.Me.GetMyOrderById;

public class GetMyOrderByIdQueryValidatorTests
{
    private readonly GetMyOrderByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetMyOrderByIdQuery(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var result = _validator.Validate(new GetMyOrderByIdQuery(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetMyOrderByIdQuery.OrderId));
    }
}
