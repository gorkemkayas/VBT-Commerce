using Catalog.Application.Queries.Categories.GetCategoryById;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Queries.Categories;

public class GetCategoryByIdQueryValidatorTests
{
    private readonly GetCategoryByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithNonEmptyCategoryId_HasNoErrors()
    {
        var result = _validator.Validate(new GetCategoryByIdQuery(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCategoryId_HasError()
    {
        var result = _validator.Validate(new GetCategoryByIdQuery(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetCategoryByIdQuery.CategoryId));
    }
}
