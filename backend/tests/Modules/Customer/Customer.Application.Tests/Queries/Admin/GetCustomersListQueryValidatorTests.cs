using Customer.Application.Queries.Admin.GetCustomersList;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Queries.Admin;

public class GetCustomersListQueryValidatorTests
{
    private readonly GetCustomersListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetCustomersListQuery(1, 20));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPageNumberBelowOne_HasError()
    {
        var result = _validator.Validate(new GetCustomersListQuery(0, 20));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetCustomersListQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutOfRange_HasError(int pageSize)
    {
        var result = _validator.Validate(new GetCustomersListQuery(1, pageSize));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetCustomersListQuery.PageSize));
    }
}
