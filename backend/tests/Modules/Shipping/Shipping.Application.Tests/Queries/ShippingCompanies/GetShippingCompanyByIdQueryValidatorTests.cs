using FluentAssertions;
using Shipping.Application.Queries.ShippingCompanies.GetShippingCompanyById;
using Xunit;

namespace Shipping.Application.Tests.Queries.ShippingCompanies;

public class GetShippingCompanyByIdQueryValidatorTests
{
    private readonly GetShippingCompanyByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetShippingCompanyByIdQuery(Guid.NewGuid());

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyShippingCompanyId_HasError()
    {
        var query = new GetShippingCompanyByIdQuery(Guid.Empty);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetShippingCompanyByIdQuery.ShippingCompanyId));
    }
}
