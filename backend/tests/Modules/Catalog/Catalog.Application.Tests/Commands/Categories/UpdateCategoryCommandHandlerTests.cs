using Catalog.Application.Commands.Categories.UpdateCategory;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Categories;

public class UpdateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_UpdatesCategory()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateCategoryCommandHandler(dbContext);
        var command = new UpdateCategoryCommand(category.Id, "Boots", "boots", "desc", "img.png", 2, null);

        await handler.Handle(command, CancellationToken.None);

        var updated = await dbContext.Categories.FindAsync(category.Id);
        updated!.Name.Should().Be("Boots");
        updated.Slug.Should().Be("boots");
        updated.DisplayOrder.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ThrowsCategoryNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new UpdateCategoryCommandHandler(dbContext);
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Boots", "boots", null, null, 0, null);

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithSlugUsedByAnotherCategory_ThrowsDuplicateCategorySlugException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        var other = Category.Create("Boots", "boots", null, null, null, 0);
        dbContext.Categories.AddRange(category, other);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateCategoryCommandHandler(dbContext);
        var command = new UpdateCategoryCommand(category.Id, "Shoes", "boots", null, null, 0, null);

        await Assert.ThrowsAsync<DuplicateCategorySlugException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithCategoryAsOwnParent_ThrowsCategoryCannotBeOwnParentException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateCategoryCommandHandler(dbContext);
        var command = new UpdateCategoryCommand(category.Id, "Shoes", "shoes-2", null, null, 0, category.Id);

        await Assert.ThrowsAsync<CategoryCannotBeOwnParentException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithCircularParentReference_ThrowsCircularCategoryReferenceException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var parent = Category.Create("Parent", "parent", null, null, null, 0);
        var child = Category.Create("Child", "child", null, null, parent.Id, 0);
        dbContext.Categories.AddRange(parent, child);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Try to set parent's parent to be its own child -> circular reference
        var handler = new UpdateCategoryCommandHandler(dbContext);
        var command = new UpdateCategoryCommand(parent.Id, "Parent", "parent", null, null, 0, child.Id);

        await Assert.ThrowsAsync<CircularCategoryReferenceException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentNewParent_ThrowsCategoryNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", null, null, null, 0);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateCategoryCommandHandler(dbContext);
        var command = new UpdateCategoryCommand(category.Id, "Shoes", "shoes", null, null, 0, Guid.NewGuid());

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
