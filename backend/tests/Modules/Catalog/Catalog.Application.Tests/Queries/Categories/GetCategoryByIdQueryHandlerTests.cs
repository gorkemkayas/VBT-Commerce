using Catalog.Application.Queries.Categories.GetCategoryById;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Queries.Categories;

public class GetCategoryByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCategory_ReturnsDto()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var category = Category.Create("Shoes", "shoes", "desc", "img.png", null, 3);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetCategoryByIdQueryHandler(dbContext);

        var dto = await handler.Handle(new GetCategoryByIdQuery(category.Id), CancellationToken.None);

        dto.Id.Should().Be(category.Id);
        dto.Name.Should().Be("Shoes");
        dto.Slug.Should().Be("shoes");
        dto.Description.Should().Be("desc");
        dto.DisplayOrder.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ThrowsCategoryNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new GetCategoryByIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => handler.Handle(new GetCategoryByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
