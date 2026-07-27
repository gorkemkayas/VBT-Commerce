using Catalog.Application.Commands.Products.SetPrimaryProductImage;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class SetPrimaryProductImageCommandValidatorTests
{
    private readonly SetPrimaryProductImageCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new SetPrimaryProductImageCommand(Guid.NewGuid(), Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var result = _validator.Validate(new SetPrimaryProductImageCommand(Guid.Empty, Guid.NewGuid()));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SetPrimaryProductImageCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyImageId_HasError()
    {
        var result = _validator.Validate(new SetPrimaryProductImageCommand(Guid.NewGuid(), Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SetPrimaryProductImageCommand.ImageId));
    }
}
