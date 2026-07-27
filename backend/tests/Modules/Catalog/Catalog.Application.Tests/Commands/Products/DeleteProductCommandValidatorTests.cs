using Catalog.Application.Commands.Products.DeleteProduct;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class DeleteProductCommandValidatorTests
{
    private readonly DeleteProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithNonEmptyProductId_HasNoErrors()
    {
        var result = _validator.Validate(new DeleteProductCommand(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var result = _validator.Validate(new DeleteProductCommand(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteProductCommand.ProductId));
    }
}
