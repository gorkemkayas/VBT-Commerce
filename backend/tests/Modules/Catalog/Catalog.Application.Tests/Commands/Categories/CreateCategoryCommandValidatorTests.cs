using Catalog.Application.Commands.Categories.CreateCategory;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Categories;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateCategoryCommand("Shoes", "shoes", "desc", null, null, 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyName_HasError(string name)
    {
        var command = new CreateCategoryCommand(name, "shoes", null, null, null, 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCategoryCommand.Name));
    }

    [Theory]
    [InlineData("Shoes")]
    [InlineData("shoes-")]
    [InlineData("-shoes")]
    [InlineData("Sh_oes")]
    public void Validate_WithInvalidSlugFormat_HasError(string slug)
    {
        var command = new CreateCategoryCommand("Shoes", slug, null, null, null, 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCategoryCommand.Slug));
    }

    [Fact]
    public void Validate_WithNegativeDisplayOrder_HasError()
    {
        var command = new CreateCategoryCommand("Shoes", "shoes", null, null, null, -1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCategoryCommand.DisplayOrder));
    }

    [Fact]
    public void Validate_WithDescriptionTooLong_HasError()
    {
        var command = new CreateCategoryCommand("Shoes", "shoes", new string('a', 2001), null, null, 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCategoryCommand.Description));
    }
}
