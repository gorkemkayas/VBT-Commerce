using Catalog.Application.Commands.Products.RemoveProductAttribute;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class RemoveProductAttributeCommandValidatorTests
{
    private readonly RemoveProductAttributeCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new RemoveProductAttributeCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var result = _validator.Validate(new RemoveProductAttributeCommand(Guid.Empty, Guid.NewGuid()));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveProductAttributeCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyAttributeId_HasError()
    {
        var result = _validator.Validate(new RemoveProductAttributeCommand(Guid.NewGuid(), Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveProductAttributeCommand.AttributeId));
    }
}
