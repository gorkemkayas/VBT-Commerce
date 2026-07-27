using Catalog.Application.Commands.Products.UpdateProductVariant;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class UpdateProductVariantCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_UpdatesVariant()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        var variant = product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductVariantCommandHandler(dbContext);
        var command = new UpdateProductVariantCommand(
            product.Id, variant.Id, "SKU-2", new Dictionary<Guid, string> { [colorAttribute.Id] = "Blue" }, true);

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.ProductVariants.FindAsync(variant.Id);
        stored!.Sku.Should().Be("SKU-2");
        var optionValues = dbContext.ProductVariantOptionValues.Where(ov => ov.ProductVariantId == variant.Id).ToList();
        optionValues.Should().ContainSingle(ov => ov.Value == "Blue");
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new UpdateProductVariantCommandHandler(dbContext);
        var command = new UpdateProductVariantCommand(
            Guid.NewGuid(), Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string>(), true);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentVariant_ThrowsProductVariantNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductVariantCommandHandler(dbContext);
        var command = new UpdateProductVariantCommand(
            product.Id, Guid.NewGuid(), "SKU-1", new Dictionary<Guid, string>(), true);

        await Assert.ThrowsAsync<ProductVariantNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithMismatchedOptionValues_ThrowsVariantAttributeMismatchException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        var variant = product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductVariantCommandHandler(dbContext);
        var command = new UpdateProductVariantCommand(
            product.Id, variant.Id, "SKU-1", new Dictionary<Guid, string> { [Guid.NewGuid()] = "Red" }, true);

        await Assert.ThrowsAsync<VariantAttributeMismatchException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithSkuUsedByAnotherVariant_ThrowsDuplicateSkuException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        var variant2 = product.AddVariant("SKU-2", new Dictionary<Guid, string> { [colorAttribute.Id] = "Blue" });
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductVariantCommandHandler(dbContext);
        var command = new UpdateProductVariantCommand(
            product.Id, variant2.Id, "sku-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Blue" }, true);

        await Assert.ThrowsAsync<DuplicateSkuException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithOptionCombinationUsedByAnotherVariant_ThrowsDuplicateVariantOptionCombinationException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        var variant2 = product.AddVariant("SKU-2", new Dictionary<Guid, string> { [colorAttribute.Id] = "Blue" });
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductVariantCommandHandler(dbContext);
        var command = new UpdateProductVariantCommand(
            product.Id, variant2.Id, "SKU-2", new Dictionary<Guid, string> { [colorAttribute.Id] = "red" }, true);

        await Assert.ThrowsAsync<DuplicateVariantOptionCombinationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithDeactivatingLastActiveVariantOfActiveProduct_ThrowsProductMustHaveAtLeastOneVariantException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        var variant = product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        product.Activate();
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductVariantCommandHandler(dbContext);
        var command = new UpdateProductVariantCommand(
            product.Id, variant.Id, "SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" }, false);

        await Assert.ThrowsAsync<ProductMustHaveAtLeastOneVariantException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
