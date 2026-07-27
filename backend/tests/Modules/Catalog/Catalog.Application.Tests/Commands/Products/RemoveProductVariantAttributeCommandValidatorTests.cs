using Catalog.Application.Commands.Products.RemoveProductVariantAttribute;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class RemoveProductVariantAttributeCommandValidatorTests
{
    private readonly RemoveProductVariantAttributeCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new RemoveProductVariantAttributeCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var result = _validator.Validate(new RemoveProductVariantAttributeCommand(Guid.Empty, Guid.NewGuid()));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveProductVariantAttributeCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyVariantAttributeId_HasError()
    {
        var result = _validator.Validate(new RemoveProductVariantAttributeCommand(Guid.NewGuid(), Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveProductVariantAttributeCommand.VariantAttributeId));
    }
}
