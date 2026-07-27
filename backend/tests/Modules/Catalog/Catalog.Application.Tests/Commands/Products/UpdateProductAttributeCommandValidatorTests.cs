using Catalog.Application.Commands.Products.UpdateProductAttribute;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class UpdateProductAttributeCommandValidatorTests
{
    private readonly UpdateProductAttributeCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new UpdateProductAttributeCommand(Guid.NewGuid(), Guid.NewGuid(), "Material", "Cotton", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var command = new UpdateProductAttributeCommand(Guid.Empty, Guid.NewGuid(), "Material", "Cotton", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductAttributeCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyAttributeId_HasError()
    {
        var command = new UpdateProductAttributeCommand(Guid.NewGuid(), Guid.Empty, "Material", "Cotton", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductAttributeCommand.AttributeId));
    }

    [Fact]
    public void Validate_WithEmptyName_HasError()
    {
        var command = new UpdateProductAttributeCommand(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Cotton", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductAttributeCommand.Name));
    }

    [Fact]
    public void Validate_WithNameTooLong_HasError()
    {
        var command = new UpdateProductAttributeCommand(Guid.NewGuid(), Guid.NewGuid(), new string('a', 101), "Cotton", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductAttributeCommand.Name));
    }

    [Fact]
    public void Validate_WithEmptyValue_HasError()
    {
        var command = new UpdateProductAttributeCommand(Guid.NewGuid(), Guid.NewGuid(), "Material", string.Empty, 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductAttributeCommand.Value));
    }

    [Fact]
    public void Validate_WithValueTooLong_HasError()
    {
        var command = new UpdateProductAttributeCommand(Guid.NewGuid(), Guid.NewGuid(), "Material", new string('a', 501), 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductAttributeCommand.Value));
    }

    [Fact]
    public void Validate_WithNegativeDisplayOrder_HasError()
    {
        var command = new UpdateProductAttributeCommand(Guid.NewGuid(), Guid.NewGuid(), "Material", "Cotton", -1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductAttributeCommand.DisplayOrder));
    }
}
