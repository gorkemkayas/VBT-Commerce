using Catalog.Application.Commands.Products.RemoveProductImage;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class RemoveProductImageCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingImage_RemovesImage()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        var image = product.AddImage("https://example.com/img.png", 0, false, null);
        dbContext.Products.Add(product);
        dbContext.ProductImages.Add(image);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new RemoveProductImageCommandHandler(dbContext);

        await handler.Handle(new RemoveProductImageCommand(product.Id, image.Id), CancellationToken.None);

        var stored = await dbContext.ProductImages.FindAsync(image.Id);
        stored.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new RemoveProductImageCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(new RemoveProductImageCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentImage_ThrowsProductImageNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new RemoveProductImageCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductImageNotFoundException>(
            () => handler.Handle(new RemoveProductImageCommand(product.Id, Guid.NewGuid()), CancellationToken.None));
    }
}
