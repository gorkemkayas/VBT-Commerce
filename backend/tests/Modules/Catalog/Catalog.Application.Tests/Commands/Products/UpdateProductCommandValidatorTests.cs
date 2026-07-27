using Catalog.Application.Commands.Products.UpdateProduct;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Boot", "boot", "desc", Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var command = new UpdateProductCommand(Guid.Empty, "Boot", "boot", null, Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyName_HasError()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), string.Empty, "boot", null, Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductCommand.Name));
    }

    [Fact]
    public void Validate_WithNameTooLong_HasError()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), new string('a', 201), "boot", null, Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductCommand.Name));
    }

    [Fact]
    public void Validate_WithInvalidSlugFormat_HasError()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Boot", "Invalid Slug!", null, Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductCommand.Slug));
    }

    [Fact]
    public void Validate_WithDescriptionTooLong_HasError()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Boot", "boot", new string('a', 4001), Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductCommand.Description));
    }

    [Fact]
    public void Validate_WithEmptyCategoryId_HasError()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Boot", "boot", null, Guid.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductCommand.CategoryId));
    }
}
