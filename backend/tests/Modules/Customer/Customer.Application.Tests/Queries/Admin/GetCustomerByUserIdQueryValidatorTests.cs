using Customer.Application.Queries.Admin.GetCustomerByUserId;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Queries.Admin;

public class GetCustomerByUserIdQueryValidatorTests
{
    private readonly GetCustomerByUserIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetCustomerByUserIdQuery(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyUserId_HasError()
    {
        var result = _validator.Validate(new GetCustomerByUserIdQuery(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetCustomerByUserIdQuery.UserId));
    }
}
