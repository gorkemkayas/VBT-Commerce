using Catalog.Application.Queries.Categories.GetCategoryTree;
using Catalog.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Queries.Categories;

public class GetCategoryTreeQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithParentAndChildCategories_BuildsNestedTree()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var parent = Category.Create("Parent", "parent", null, null, null, 0);
        var child = Category.Create("Child", "child", null, null, parent.Id, 0);
        dbContext.Categories.AddRange(parent, child);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetCategoryTreeQueryHandler(dbContext);

        var tree = await handler.Handle(new GetCategoryTreeQuery(), CancellationToken.None);

        tree.Should().HaveCount(1);
        var parentNode = tree.Single();
        parentNode.Id.Should().Be(parent.Id);
        parentNode.Children.Should().ContainSingle(c => c.Id == child.Id);
    }

    [Fact]
    public async Task Handle_WithIncludeInactiveFalse_ExcludesInactiveCategories()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var active = Category.Create("Active", "active", null, null, null, 0);
        var inactive = Category.Create("Inactive", "inactive", null, null, null, 1);
        inactive.Deactivate();
        dbContext.Categories.AddRange(active, inactive);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetCategoryTreeQueryHandler(dbContext);

        var tree = await handler.Handle(new GetCategoryTreeQuery(IncludeInactive: false), CancellationToken.None);

        tree.Should().ContainSingle(c => c.Id == active.Id);
    }

    [Fact]
    public async Task Handle_WithIncludeInactiveTrue_IncludesInactiveCategories()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var active = Category.Create("Active", "active", null, null, null, 0);
        var inactive = Category.Create("Inactive", "inactive", null, null, null, 1);
        inactive.Deactivate();
        dbContext.Categories.AddRange(active, inactive);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetCategoryTreeQueryHandler(dbContext);

        var tree = await handler.Handle(new GetCategoryTreeQuery(IncludeInactive: true), CancellationToken.None);

        tree.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoCategories_ReturnsEmptyCollection()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new GetCategoryTreeQueryHandler(dbContext);

        var tree = await handler.Handle(new GetCategoryTreeQuery(), CancellationToken.None);

        tree.Should().BeEmpty();
    }
}
