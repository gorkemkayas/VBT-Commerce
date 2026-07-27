using Catalog.Application.Commands.Products.AddProductVariant;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class AddProductVariantCommandValidatorTests
{
    private readonly AddProductVariantCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new AddProductVariantCommand(
            Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string> { [Guid.NewGuid()] = "Red" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var command = new AddProductVariantCommand(Guid.Empty, "SKU-1", new Dictionary<Guid, string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductVariantCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptySku_HasError()
    {
        var command = new AddProductVariantCommand(Guid.NewGuid(), string.Empty, new Dictionary<Guid, string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductVariantCommand.Sku));
    }

    [Fact]
    public void Validate_WithSkuTooLong_HasError()
    {
        var command = new AddProductVariantCommand(Guid.NewGuid(), new string('a', 101), new Dictionary<Guid, string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductVariantCommand.Sku));
    }

    [Fact]
    public void Validate_WithNullOptionValues_HasError()
    {
        var command = new AddProductVariantCommand(Guid.NewGuid(), "SKU-1", null!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddProductVariantCommand.OptionValues));
    }

    [Fact]
    public void Validate_WithEmptyOptionValueKey_HasError()
    {
        var command = new AddProductVariantCommand(
            Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string> { [Guid.Empty] = "Red" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithEmptyOptionValueValue_HasError()
    {
        var command = new AddProductVariantCommand(
            Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string> { [Guid.NewGuid()] = string.Empty });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithOptionValueTooLong_HasError()
    {
        var command = new AddProductVariantCommand(
            Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string> { [Guid.NewGuid()] = new string('a', 201) });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
