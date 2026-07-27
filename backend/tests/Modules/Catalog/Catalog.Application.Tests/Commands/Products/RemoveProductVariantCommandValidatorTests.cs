using Catalog.Application.Commands.Products.RemoveProductVariant;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class RemoveProductVariantCommandValidatorTests
{
    private readonly RemoveProductVariantCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new RemoveProductVariantCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var result = _validator.Validate(new RemoveProductVariantCommand(Guid.Empty, Guid.NewGuid()));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveProductVariantCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyVariantId_HasError()
    {
        var result = _validator.Validate(new RemoveProductVariantCommand(Guid.NewGuid(), Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveProductVariantCommand.VariantId));
    }
}
