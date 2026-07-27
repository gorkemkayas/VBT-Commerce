using Catalog.Application.Queries.Products.GetProductsList;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Queries.Products;

public class GetProductsListQueryValidatorTests
{
    private readonly GetProductsListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetProductsListQuery(1, 20));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPageNumberLessThanOne_HasError()
    {
        var result = _validator.Validate(new GetProductsListQuery(PageNumber: 0));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductsListQuery.PageNumber));
    }

    [Fact]
    public void Validate_WithPageSizeZero_HasError()
    {
        var result = _validator.Validate(new GetProductsListQuery(PageSize: 0));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductsListQuery.PageSize));
    }

    [Fact]
    public void Validate_WithPageSizeTooLarge_HasError()
    {
        var result = _validator.Validate(new GetProductsListQuery(PageSize: 101));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductsListQuery.PageSize));
    }
}
