using Catalog.Application.Commands.Products.UpdateProduct;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_UpdatesProduct()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        var product = Product.Create("Sneaker", "sneaker", null, category.Id, ProductType.Simple);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductCommandHandler(dbContext);
        var command = new UpdateProductCommand(product.Id, "Boot", "boot", "desc", category.Id);

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Products.FindAsync(product.Id);
        stored!.Name.Should().Be("Boot");
        stored.Slug.Should().Be("boot");
        stored.Description.Should().Be("desc");
    }

    [Fact]
    public async Task Handle_WithSameSlugAsOwnProduct_DoesNotThrow()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        var product = Product.Create("Sneaker", "sneaker", null, category.Id, ProductType.Simple);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductCommandHandler(dbContext);
        var command = new UpdateProductCommand(product.Id, "Sneaker", "sneaker", "new desc", category.Id);

        await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Products.FindAsync(product.Id);
        stored!.Description.Should().Be("new desc");
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new UpdateProductCommandHandler(dbContext);
        var command = new UpdateProductCommand(Guid.NewGuid(), "Boot", "boot", null, Guid.NewGuid());

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ThrowsCategoryNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        var product = Product.Create("Sneaker", "sneaker", null, category.Id, ProductType.Simple);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductCommandHandler(dbContext);
        var command = new UpdateProductCommand(product.Id, "Boot", "boot", null, Guid.NewGuid());

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithSlugUsedByAnotherProduct_ThrowsDuplicateProductSlugException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        var product = Product.Create("Sneaker", "sneaker", null, category.Id, ProductType.Simple);
        var other = Product.Create("Boot", "boot", null, category.Id, ProductType.Simple);
        dbContext.Categories.Add(category);
        dbContext.Products.AddRange(product, other);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateProductCommandHandler(dbContext);
        var command = new UpdateProductCommand(product.Id, "Sneaker", "boot", null, category.Id);

        await Assert.ThrowsAsync<DuplicateProductSlugException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
