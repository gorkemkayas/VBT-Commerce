using FluentAssertions;
using Order.Application.Queries.Admin.GetOrderById;
using Xunit;

namespace Order.Application.Tests.Queries.Admin.GetOrderById;

public class GetOrderByIdQueryValidatorTests
{
    private readonly GetOrderByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetOrderByIdQuery(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var result = _validator.Validate(new GetOrderByIdQuery(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetOrderByIdQuery.OrderId));
    }
}
