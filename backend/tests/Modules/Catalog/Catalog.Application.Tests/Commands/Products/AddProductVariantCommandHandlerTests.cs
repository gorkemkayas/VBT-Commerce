using Catalog.Application.Commands.Products.AddProductVariant;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class AddProductVariantCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithMatchingOptionValues_AddsVariant()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductVariantCommandHandler(dbContext);
        var command = new AddProductVariantCommand(
            product.Id, "SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });

        var variantId = await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.ProductVariants.FindAsync(variantId);
        stored.Should().NotBeNull();
        stored!.Sku.Should().Be("SKU-1");
        var optionValues = dbContext.ProductVariantOptionValues.Where(ov => ov.ProductVariantId == variantId).ToList();
        optionValues.Should().ContainSingle(ov => ov.Value == "Red");
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new AddProductVariantCommandHandler(dbContext);
        var command = new AddProductVariantCommand(Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string>());

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithSimpleTypeProduct_ThrowsInvalidProductTypeOperationException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductVariantCommandHandler(dbContext);
        var command = new AddProductVariantCommand(product.Id, "SKU-1", new Dictionary<Guid, string>());

        await Assert.ThrowsAsync<InvalidProductTypeOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithMismatchedOptionValues_ThrowsVariantAttributeMismatchException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        product.AddVariantAttribute("Color", 0);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductVariantCommandHandler(dbContext);
        var command = new AddProductVariantCommand(
            product.Id, "SKU-1", new Dictionary<Guid, string> { [Guid.NewGuid()] = "Red" });

        await Assert.ThrowsAsync<VariantAttributeMismatchException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithDuplicateSku_ThrowsDuplicateSkuException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductVariantCommandHandler(dbContext);
        var command = new AddProductVariantCommand(
            product.Id, "sku-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Blue" });

        await Assert.ThrowsAsync<DuplicateSkuException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithDuplicateOptionCombination_ThrowsDuplicateVariantOptionCombinationException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductVariantCommandHandler(dbContext);
        var command = new AddProductVariantCommand(
            product.Id, "SKU-2", new Dictionary<Guid, string> { [colorAttribute.Id] = "red" });

        await Assert.ThrowsAsync<DuplicateVariantOptionCombinationException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
