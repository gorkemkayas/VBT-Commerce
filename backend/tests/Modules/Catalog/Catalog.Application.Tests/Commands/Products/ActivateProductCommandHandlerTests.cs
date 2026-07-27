using Catalog.Application.Commands.Products.ActivateProduct;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class ActivateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithSimpleProduct_ActivatesProduct()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ActivateProductCommandHandler(dbContext);

        await handler.Handle(new ActivateProductCommand(product.Id), CancellationToken.None);

        var stored = await dbContext.Products.FindAsync(product.Id);
        stored!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithVariantProductHavingActiveVariant_ActivatesProduct()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ActivateProductCommandHandler(dbContext);

        await handler.Handle(new ActivateProductCommand(product.Id), CancellationToken.None);

        var stored = await dbContext.Products.FindAsync(product.Id);
        stored!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithVariantProductHavingNoActiveVariant_ThrowsProductMustHaveAtLeastOneVariantException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new ActivateProductCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductMustHaveAtLeastOneVariantException>(
            () => handler.Handle(new ActivateProductCommand(product.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new ActivateProductCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(new ActivateProductCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
