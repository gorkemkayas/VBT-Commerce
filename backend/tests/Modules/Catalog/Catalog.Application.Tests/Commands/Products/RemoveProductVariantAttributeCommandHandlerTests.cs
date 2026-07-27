using Catalog.Application.Commands.Products.RemoveProductVariantAttribute;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class RemoveProductVariantAttributeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithUnusedVariantAttribute_RemovesIt()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new RemoveProductVariantAttributeCommandHandler(dbContext);

        await handler.Handle(
            new RemoveProductVariantAttributeCommand(product.Id, colorAttribute.Id), CancellationToken.None);

        var stored = await dbContext.ProductVariantAttributes.FindAsync(colorAttribute.Id);
        stored.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new RemoveProductVariantAttributeCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(
                new RemoveProductVariantAttributeCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentVariantAttribute_ThrowsProductVariantAttributeNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new RemoveProductVariantAttributeCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductVariantAttributeNotFoundException>(
            () => handler.Handle(
                new RemoveProductVariantAttributeCommand(product.Id, Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithVariantAttributeInUse_ThrowsVariantAttributeInUseException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new RemoveProductVariantAttributeCommandHandler(dbContext);

        await Assert.ThrowsAsync<VariantAttributeInUseException>(
            () => handler.Handle(
                new RemoveProductVariantAttributeCommand(product.Id, colorAttribute.Id), CancellationToken.None));
    }
}
