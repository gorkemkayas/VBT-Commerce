using Catalog.Application.Commands.Products.CreateProduct;
using Catalog.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateProductCommand("Sneaker", "sneaker", "desc", Guid.NewGuid(), ProductType.Simple);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_HasError()
    {
        var command = new CreateProductCommand(string.Empty, "sneaker", null, Guid.NewGuid(), ProductType.Simple);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public void Validate_WithNameTooLong_HasError()
    {
        var command = new CreateProductCommand(new string('a', 201), "sneaker", null, Guid.NewGuid(), ProductType.Simple);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Sneaker")]
    [InlineData("sneaker-")]
    [InlineData("-sneaker")]
    [InlineData("sn_eaker")]
    public void Validate_WithInvalidSlugFormat_HasError(string slug)
    {
        var command = new CreateProductCommand("Sneaker", slug, null, Guid.NewGuid(), ProductType.Simple);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Slug));
    }

    [Fact]
    public void Validate_WithSlugTooLong_HasError()
    {
        var command = new CreateProductCommand("Sneaker", new string('a', 201), null, Guid.NewGuid(), ProductType.Simple);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Slug));
    }

    [Fact]
    public void Validate_WithDescriptionTooLong_HasError()
    {
        var command = new CreateProductCommand("Sneaker", "sneaker", new string('a', 4001), Guid.NewGuid(), ProductType.Simple);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Description));
    }

    [Fact]
    public void Validate_WithEmptyCategoryId_HasError()
    {
        var command = new CreateProductCommand("Sneaker", "sneaker", null, Guid.Empty, ProductType.Simple);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.CategoryId));
    }

    [Fact]
    public void Validate_WithInvalidProductType_HasError()
    {
        var command = new CreateProductCommand("Sneaker", "sneaker", null, Guid.NewGuid(), (ProductType)99);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.ProductType));
    }
}
