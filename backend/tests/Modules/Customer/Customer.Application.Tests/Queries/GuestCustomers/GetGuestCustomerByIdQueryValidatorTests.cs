using Customer.Application.Queries.GuestCustomers.GetGuestCustomerById;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Queries.GuestCustomers;

public class GetGuestCustomerByIdQueryValidatorTests
{
    private readonly GetGuestCustomerByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetGuestCustomerByIdQuery(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyGuestCustomerId_HasError()
    {
        var result = _validator.Validate(new GetGuestCustomerByIdQuery(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetGuestCustomerByIdQuery.GuestCustomerId));
    }
}
