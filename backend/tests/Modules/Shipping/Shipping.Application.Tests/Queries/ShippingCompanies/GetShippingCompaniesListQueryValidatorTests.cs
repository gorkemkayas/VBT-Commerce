using FluentAssertions;
using Shipping.Application.Queries.ShippingCompanies.GetShippingCompaniesList;
using Xunit;

namespace Shipping.Application.Tests.Queries.ShippingCompanies;

public class GetShippingCompaniesListQueryValidatorTests
{
    private readonly GetShippingCompaniesListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetShippingCompaniesListQuery(1, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPageNumberLessThanOne_HasError()
    {
        var query = new GetShippingCompaniesListQuery(0, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetShippingCompaniesListQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutOfRange_HasError(int pageSize)
    {
        var query = new GetShippingCompaniesListQuery(1, pageSize);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetShippingCompaniesListQuery.PageSize));
    }
}
