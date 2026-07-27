using Catalog.Application.Commands.Categories.CreateCategory;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Categories;

public class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithUniqueSlugAndNoParent_CreatesCategory()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new CreateCategoryCommandHandler(dbContext);
        var command = new CreateCategoryCommand("Shoes", "shoes", "Footwear", null, null, 1);

        var categoryId = await handler.Handle(command, CancellationToken.None);

        categoryId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.Categories.FindAsync(categoryId);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Shoes");
        stored.Slug.Should().Be("shoes");
        stored.ParentCategoryId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithExistingParent_SetsParentCategoryId()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var parent = Category.Create("Clothing", "clothing", null, null, null, 0);
        dbContext.Categories.Add(parent);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateCategoryCommandHandler(dbContext);
        var command = new CreateCategoryCommand("Shirts", "shirts", null, null, parent.Id, 1);

        var categoryId = await handler.Handle(command, CancellationToken.None);

        var stored = await dbContext.Categories.FindAsync(categoryId);
        stored!.ParentCategoryId.Should().Be(parent.Id);
    }

    [Fact]
    public async Task Handle_WithDuplicateSlug_ThrowsDuplicateCategorySlugException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        dbContext.Categories.Add(Category.Create("Shoes", "shoes", null, null, null, 0));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateCategoryCommandHandler(dbContext);
        var command = new CreateCategoryCommand("Other Shoes", "SHOES", null, null, null, 1);

        await Assert.ThrowsAsync<DuplicateCategorySlugException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentParent_ThrowsCategoryNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new CreateCategoryCommandHandler(dbContext);
        var command = new CreateCategoryCommand("Shirts", "shirts", null, null, Guid.NewGuid(), 1);

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
