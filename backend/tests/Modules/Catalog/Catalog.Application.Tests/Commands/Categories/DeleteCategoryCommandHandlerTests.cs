using Catalog.Application.Commands.Categories.DeleteCategory;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Categories;

public class DeleteCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNoActiveChildrenOrProducts_DeactivatesCategory()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteCategoryCommandHandler(dbContext);

        await handler.Handle(new DeleteCategoryCommand(category.Id), CancellationToken.None);

        var stored = await dbContext.Categories.FindAsync(category.Id);
        stored!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ThrowsCategoryNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new DeleteCategoryCommandHandler(dbContext);

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => handler.Handle(new DeleteCategoryCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithActiveChildCategory_ThrowsCategoryHasActiveChildrenException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var parent = Category.Create("Parent", "parent", null, null, null, 0);
        var child = Category.Create("Child", "child", null, null, parent.Id, 0);
        dbContext.Categories.AddRange(parent, child);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteCategoryCommandHandler(dbContext);

        await Assert.ThrowsAsync<CategoryHasActiveChildrenException>(
            () => handler.Handle(new DeleteCategoryCommand(parent.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithActiveProduct_ThrowsCategoryHasActiveProductsException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        var product = Product.Create("Sneaker", "sneaker", null, category.Id, ProductType.Simple);
        product.Activate();
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteCategoryCommandHandler(dbContext);

        await Assert.ThrowsAsync<CategoryHasActiveProductsException>(
            () => handler.Handle(new DeleteCategoryCommand(category.Id), CancellationToken.None));
    }
}
