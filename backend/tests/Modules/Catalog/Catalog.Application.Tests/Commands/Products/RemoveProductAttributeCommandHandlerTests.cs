using Catalog.Application.Commands.Products.RemoveProductAttribute;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class RemoveProductAttributeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAttribute_RemovesAttribute()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        var attribute = product.AddAttribute("Material", "Cotton", 0);
        dbContext.Products.Add(product);
        dbContext.ProductAttributes.Add(attribute);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new RemoveProductAttributeCommandHandler(dbContext);

        await handler.Handle(new RemoveProductAttributeCommand(product.Id, attribute.Id), CancellationToken.None);

        var stored = await dbContext.ProductAttributes.FindAsync(attribute.Id);
        stored.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new RemoveProductAttributeCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(new RemoveProductAttributeCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentAttribute_ThrowsProductAttributeNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new RemoveProductAttributeCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductAttributeNotFoundException>(
            () => handler.Handle(new RemoveProductAttributeCommand(product.Id, Guid.NewGuid()), CancellationToken.None));
    }
}
