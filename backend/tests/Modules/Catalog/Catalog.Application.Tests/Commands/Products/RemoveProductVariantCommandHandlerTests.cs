using Catalog.Application.Commands.Products.RemoveProductVariant;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class RemoveProductVariantCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithInactiveProduct_RemovesVariant()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        var variant = product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new RemoveProductVariantCommandHandler(dbContext);

        await handler.Handle(new RemoveProductVariantCommand(product.Id, variant.Id), CancellationToken.None);

        var stored = await dbContext.ProductVariants.FindAsync(variant.Id);
        stored.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new RemoveProductVariantCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(new RemoveProductVariantCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentVariant_ThrowsProductVariantNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new RemoveProductVariantCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductVariantNotFoundException>(
            () => handler.Handle(new RemoveProductVariantCommand(product.Id, Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithLastActiveVariantOfActiveProduct_ThrowsProductMustHaveAtLeastOneVariantException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        var variant = product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        product.Activate();
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new RemoveProductVariantCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductMustHaveAtLeastOneVariantException>(
            () => handler.Handle(new RemoveProductVariantCommand(product.Id, variant.Id), CancellationToken.None));
    }
}
