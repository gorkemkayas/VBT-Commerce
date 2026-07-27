using Catalog.Application.Commands.Products.SetPrimaryProductImage;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class SetPrimaryProductImageCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingImage_SetsPrimaryAndUnsetsPrevious()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        var firstImage = product.AddImage("https://example.com/first.png", 0, true, null);
        var secondImage = product.AddImage("https://example.com/second.png", 1, false, null);
        dbContext.Products.Add(product);
        dbContext.ProductImages.AddRange(firstImage, secondImage);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new SetPrimaryProductImageCommandHandler(dbContext);

        await handler.Handle(new SetPrimaryProductImageCommand(product.Id, secondImage.Id), CancellationToken.None);

        var storedFirst = await dbContext.ProductImages.FindAsync(firstImage.Id);
        var storedSecond = await dbContext.ProductImages.FindAsync(secondImage.Id);
        storedFirst!.IsPrimary.Should().BeFalse();
        storedSecond!.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new SetPrimaryProductImageCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(new SetPrimaryProductImageCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentImage_ThrowsProductImageNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new SetPrimaryProductImageCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductImageNotFoundException>(
            () => handler.Handle(new SetPrimaryProductImageCommand(product.Id, Guid.NewGuid()), CancellationToken.None));
    }
}
