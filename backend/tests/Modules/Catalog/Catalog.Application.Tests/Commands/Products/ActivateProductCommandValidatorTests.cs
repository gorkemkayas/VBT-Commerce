using Catalog.Application.Commands.Products.ActivateProduct;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class ActivateProductCommandValidatorTests
{
    private readonly ActivateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithNonEmptyProductId_HasNoErrors()
    {
        var result = _validator.Validate(new ActivateProductCommand(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var result = _validator.Validate(new ActivateProductCommand(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ActivateProductCommand.ProductId));
    }
}
