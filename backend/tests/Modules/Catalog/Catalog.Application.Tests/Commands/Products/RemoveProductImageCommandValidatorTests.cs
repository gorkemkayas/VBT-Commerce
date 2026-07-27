using Catalog.Application.Commands.Products.RemoveProductImage;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class RemoveProductImageCommandValidatorTests
{
    private readonly RemoveProductImageCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new RemoveProductImageCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var result = _validator.Validate(new RemoveProductImageCommand(Guid.Empty, Guid.NewGuid()));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveProductImageCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyImageId_HasError()
    {
        var result = _validator.Validate(new RemoveProductImageCommand(Guid.NewGuid(), Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveProductImageCommand.ImageId));
    }
}
