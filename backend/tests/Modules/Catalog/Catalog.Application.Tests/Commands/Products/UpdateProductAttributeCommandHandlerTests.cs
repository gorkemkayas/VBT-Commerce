using Catalog.Application.Commands.Products.UpdateProductAttribute;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class UpdateProductAttributeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAttribute_UpdatesAttribute()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        var attribute = product.AddAttribute("Material", "Cotton", 0);
        dbContext.Products.Add(product);
        dbContext.ProductAttributes.Add(attribute);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductAttributeCommandHandler(dbContext);
        var command = new UpdateProductAttributeCommand(product.Id, attribute.Id, "Material", "Wool", 1);

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.ProductAttributes.FindAsync(attribute.Id);
        stored!.Value.Should().Be("Wool");
        stored.DisplayOrder.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new UpdateProductAttributeCommandHandler(dbContext);
        var command = new UpdateProductAttributeCommand(Guid.NewGuid(), Guid.NewGuid(), "Material", "Wool", 0);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentAttribute_ThrowsProductAttributeNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductAttributeCommandHandler(dbContext);
        var command = new UpdateProductAttributeCommand(product.Id, Guid.NewGuid(), "Material", "Wool", 0);

        await Assert.ThrowsAsync<ProductAttributeNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
