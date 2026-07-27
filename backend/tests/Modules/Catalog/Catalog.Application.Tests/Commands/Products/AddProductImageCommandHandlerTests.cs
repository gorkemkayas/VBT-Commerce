using Catalog.Application.Commands.Products.AddProductImage;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class AddProductImageCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNoVariantAssociation_AddsImage()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductImageCommandHandler(dbContext);
        var command = new AddProductImageCommand(product.Id, "https://example.com/img.png", 0, false, null);

        var imageId = await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.ProductImages.FindAsync(imageId);
        stored.Should().NotBeNull();
        stored!.Url.Should().Be("https://example.com/img.png");
        stored.ProductVariantId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithExistingVariantId_AssociatesImageWithVariant()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        var variant = product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductImageCommandHandler(dbContext);
        var command = new AddProductImageCommand(product.Id, "https://example.com/red.png", 0, false, variant.Id);

        var imageId = await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.ProductImages.FindAsync(imageId);
        stored!.ProductVariantId.Should().Be(variant.Id);
    }

    [Fact]
    public async Task Handle_WithNonExistentVariantId_ThrowsProductVariantNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", null, Guid.NewGuid(), ProductType.Variant);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductImageCommandHandler(dbContext);
        var command = new AddProductImageCommand(product.Id, "https://example.com/red.png", 0, false, Guid.NewGuid());

        await Assert.ThrowsAsync<ProductVariantNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithIsPrimaryTrue_UnsetsPreviousPrimaryImage()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        var firstImage = product.AddImage("https://example.com/first.png", 0, true, null);
        dbContext.Products.Add(product);
        dbContext.ProductImages.Add(firstImage);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductImageCommandHandler(dbContext);
        var command = new AddProductImageCommand(product.Id, "https://example.com/second.png", 1, true, null);

        var secondImageId = await handler.Handle(command, CancellationToken.None);

        var storedFirst = await dbContext.ProductImages.FindAsync(firstImage.Id);
        var storedSecond = await dbContext.ProductImages.FindAsync(secondImageId);
        storedFirst!.IsPrimary.Should().BeFalse();
        storedSecond!.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new AddProductImageCommandHandler(dbContext);
        var command = new AddProductImageCommand(Guid.NewGuid(), "https://example.com/img.png", 0, false, null);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
