using Catalog.Application.Commands.Products.AddProductVariantAttribute;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class AddProductVariantAttributeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithVariantTypeProduct_AddsVariantAttribute()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductVariantAttributeCommandHandler(dbContext);
        var command = new AddProductVariantAttributeCommand(product.Id, "Color", 0);

        var attributeId = await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.ProductVariantAttributes.FindAsync(attributeId);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Color");
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new AddProductVariantAttributeCommandHandler(dbContext);
        var command = new AddProductVariantAttributeCommand(Guid.NewGuid(), "Color", 0);

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

        var handler = new AddProductVariantAttributeCommandHandler(dbContext);
        var command = new AddProductVariantAttributeCommand(product.Id, "Color", 0);

        await Assert.ThrowsAsync<InvalidProductTypeOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
