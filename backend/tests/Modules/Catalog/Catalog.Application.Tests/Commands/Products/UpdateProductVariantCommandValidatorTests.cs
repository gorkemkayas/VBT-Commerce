using Catalog.Application.Commands.Products.UpdateProductVariant;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class UpdateProductVariantCommandValidatorTests
{
    private readonly UpdateProductVariantCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new UpdateProductVariantCommand(
            Guid.NewGuid(), Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string> { [Guid.NewGuid()] = "Red" }, true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyProductId_HasError()
    {
        var command = new UpdateProductVariantCommand(
            Guid.Empty, Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string>(), true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductVariantCommand.ProductId));
    }

    [Fact]
    public void Validate_WithEmptyVariantId_HasError()
    {
        var command = new UpdateProductVariantCommand(
            Guid.NewGuid(), Guid.Empty, "SKU-1", new Dictionary<Guid, string>(), true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductVariantCommand.VariantId));
    }

    [Fact]
    public void Validate_WithEmptySku_HasError()
    {
        var command = new UpdateProductVariantCommand(
            Guid.NewGuid(), Guid.NewGuid(), string.Empty, new Dictionary<Guid, string>(), true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductVariantCommand.Sku));
    }

    [Fact]
    public void Validate_WithSkuTooLong_HasError()
    {
        var command = new UpdateProductVariantCommand(
            Guid.NewGuid(), Guid.NewGuid(), new string('a', 101), new Dictionary<Guid, string>(), true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductVariantCommand.Sku));
    }

    [Fact]
    public void Validate_WithNullOptionValues_HasError()
    {
        var command = new UpdateProductVariantCommand(Guid.NewGuid(), Guid.NewGuid(), "SKU-1", null!, true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductVariantCommand.OptionValues));
    }

    [Fact]
    public void Validate_WithEmptyOptionValueKey_HasError()
    {
        var command = new UpdateProductVariantCommand(
            Guid.NewGuid(), Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string> { [Guid.Empty] = "Red" }, true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithOptionValueTooLong_HasError()
    {
        var command = new UpdateProductVariantCommand(
            Guid.NewGuid(), Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string> { [Guid.NewGuid()] = new string('a', 201) }, true);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
