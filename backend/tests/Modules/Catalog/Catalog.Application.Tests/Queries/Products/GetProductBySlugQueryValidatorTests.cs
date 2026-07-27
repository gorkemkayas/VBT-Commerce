using Catalog.Application.Queries.Products.GetProductBySlug;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Queries.Products;

public class GetProductBySlugQueryValidatorTests
{
    private readonly GetProductBySlugQueryValidator _validator = new();

    [Fact]
    public void Validate_WithNonEmptySlug_HasNoErrors()
    {
        var result = _validator.Validate(new GetProductBySlugQuery("shirt"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySlug_HasError()
    {
        var result = _validator.Validate(new GetProductBySlugQuery(string.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductBySlugQuery.Slug));
    }
}
