using Catalog.Application.Commands.Products.AddProductVariantAttribute;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class AddProductVariantAttributeCommandValidatorTests
{
    private readonly AddProductVariantAttributeCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new AddProductVariantAttributeCommand(Guid.NewGuid(), "Color", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var command = new AddProductVariantAttributeCommand(Guid.Empty, "Color", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductVariantAttributeCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyName_HasError()
    {
        var command = new AddProductVariantAttributeCommand(Guid.NewGuid(), string.Empty, 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductVariantAttributeCommand.Name));
    }

    [Fact]
    public void Validate_WithNameTooLong_HasError()
    {
        var command = new AddProductVariantAttributeCommand(Guid.NewGuid(), new string('a', 101), 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductVariantAttributeCommand.Name));
    }

    [Fact]
    public void Validate_WithNegativeDisplayOrder_HasError()
    {
        var command = new AddProductVariantAttributeCommand(Guid.NewGuid(), "Color", -1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductVariantAttributeCommand.DisplayOrder));
    }
}
