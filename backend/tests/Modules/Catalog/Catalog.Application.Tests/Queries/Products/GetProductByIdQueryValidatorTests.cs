using Catalog.Application.Queries.Products.GetProductById;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Queries.Products;

public class GetProductByIdQueryValidatorTests
{
    private readonly GetProductByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithNonEmptyProductId_HasNoErrors()
    {
        var result = _validator.Validate(new GetProductByIdQuery(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var result = _validator.Validate(new GetProductByIdQuery(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductByIdQuery.ProductId));
    }
}
