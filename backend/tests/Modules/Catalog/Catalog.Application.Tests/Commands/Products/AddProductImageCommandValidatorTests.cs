using Catalog.Application.Commands.Products.AddProductImage;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class AddProductImageCommandValidatorTests
{
    private readonly AddProductImageCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new AddProductImageCommand(Guid.NewGuid(), "https://example.com/img.png", 0, false, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var command = new AddProductImageCommand(Guid.Empty, "https://example.com/img.png", 0, false, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductImageCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyUrl_HasError()
    {
        var command = new AddProductImageCommand(Guid.NewGuid(), string.Empty, 0, false, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductImageCommand.Url));
    }

    [Fact]
    public void Validate_WithUrlTooLong_HasError()
    {
        var command = new AddProductImageCommand(Guid.NewGuid(), new string('a', 2001), 0, false, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductImageCommand.Url));
    }

    [Fact]
    public void Validate_WithNegativeDisplayOrder_HasError()
    {
        var command = new AddProductImageCommand(Guid.NewGuid(), "https://example.com/img.png", -1, false, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductImageCommand.DisplayOrder));
    }
}
