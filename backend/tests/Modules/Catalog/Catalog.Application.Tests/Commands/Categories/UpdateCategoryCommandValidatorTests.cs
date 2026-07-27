using Catalog.Application.Commands.Categories.UpdateCategory;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Categories;

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Shoes", "shoes", null, null, 0, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCategoryId_HasError()
    {
        var command = new UpdateCategoryCommand(Guid.Empty, "Shoes", "shoes", null, null, 0, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateCategoryCommand.CategoryId));
    }

    [Fact]
    public void Validate_WithNameTooLong_HasError()
    {
        var command = new UpdateCategoryCommand(Guid.NewGuid(), new string('a', 151), "shoes", null, null, 0, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateCategoryCommand.Name));
    }

    [Fact]
    public void Validate_WithInvalidSlug_HasError()
    {
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Shoes", "Invalid Slug!", null, null, 0, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateCategoryCommand.Slug));
    }
}
