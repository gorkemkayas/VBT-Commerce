using Catalog.Application.Commands.Categories.DeleteCategory;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Categories;

public class DeleteCategoryCommandValidatorTests
{
    private readonly DeleteCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_WithNonEmptyCategoryId_HasNoErrors()
    {
        var result = _validator.Validate(new DeleteCategoryCommand(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCategoryId_HasError()
    {
        var result = _validator.Validate(new DeleteCategoryCommand(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteCategoryCommand.CategoryId));
    }
}
