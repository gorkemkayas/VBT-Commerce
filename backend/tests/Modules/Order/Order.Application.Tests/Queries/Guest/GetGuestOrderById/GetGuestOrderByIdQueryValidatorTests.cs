using FluentAssertions;
using Order.Application.Queries.Guest.GetGuestOrderById;
using Xunit;

namespace Order.Application.Tests.Queries.Guest.GetGuestOrderById;

public class GetGuestOrderByIdQueryValidatorTests
{
    private readonly GetGuestOrderByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetGuestOrderByIdQuery(Guid.NewGuid(), Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyGuestCustomerId_HasError()
    {
        var result = _validator.Validate(new GetGuestOrderByIdQuery(Guid.Empty, Guid.NewGuid()));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetGuestOrderByIdQuery.GuestCustomerId));
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var result = _validator.Validate(new GetGuestOrderByIdQuery(Guid.NewGuid(), Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetGuestOrderByIdQuery.OrderId));
    }
}
