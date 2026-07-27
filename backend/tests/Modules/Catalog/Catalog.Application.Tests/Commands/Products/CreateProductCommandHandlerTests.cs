using Catalog.Application.Commands.Products.CreateProduct;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCategoryAndUniqueSlug_CreatesProduct()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateProductCommandHandler(dbContext);
        var command = new CreateProductCommand("Sneaker", "sneaker", "desc", category.Id, ProductType.Simple);

        var productId = await handler.Handle(command, CancellationToken.None);

        productId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.Products.FindAsync(productId);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Sneaker");
        stored.Slug.Should().Be("sneaker");
        stored.CategoryId.Should().Be(category.Id);
        stored.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ThrowsCategoryNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new CreateProductCommandHandler(dbContext);
        var command = new CreateProductCommand("Sneaker", "sneaker", null, Guid.NewGuid(), ProductType.Simple);

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithDuplicateSlug_ThrowsDuplicateProductSlugException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(Product.Create("Sneaker", "sneaker", null, category.Id, ProductType.Simple));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateProductCommandHandler(dbContext);
        var command = new CreateProductCommand("Other Sneaker", "SNEAKER", null, category.Id, ProductType.Simple);

        await Assert.ThrowsAsync<DuplicateProductSlugException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
