using Catalog.Application.Commands.Products.AddProductAttribute;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class AddProductAttributeCommandValidatorTests
{
    private readonly AddProductAttributeCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new AddProductAttributeCommand(Guid.NewGuid(), "Material", "Cotton", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var command = new AddProductAttributeCommand(Guid.Empty, "Material", "Cotton", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductAttributeCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyName_HasError()
    {
        var command = new AddProductAttributeCommand(Guid.NewGuid(), string.Empty, "Cotton", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductAttributeCommand.Name));
    }

    [Fact]
    public void Validate_WithNameTooLong_HasError()
    {
        var command = new AddProductAttributeCommand(Guid.NewGuid(), new string('a', 101), "Cotton", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductAttributeCommand.Name));
    }

    [Fact]
    public void Validate_WithEmptyValue_HasError()
    {
        var command = new AddProductAttributeCommand(Guid.NewGuid(), "Material", string.Empty, 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductAttributeCommand.Value));
    }

    [Fact]
    public void Validate_WithValueTooLong_HasError()
    {
        var command = new AddProductAttributeCommand(Guid.NewGuid(), "Material", new string('a', 501), 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductAttributeCommand.Value));
    }

    [Fact]
    public void Validate_WithNegativeDisplayOrder_HasError()
    {
        var command = new AddProductAttributeCommand(Guid.NewGuid(), "Material", "Cotton", -1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductAttributeCommand.DisplayOrder));
    }
}
