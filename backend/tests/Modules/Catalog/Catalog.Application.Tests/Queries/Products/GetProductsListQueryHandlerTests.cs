using Catalog.Application.Queries.Products.GetProductsList;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Queries.Products;

public class GetProductsListQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoFilters_ReturnsAllProductsOrderedByName()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var categoryId = Guid.NewGuid();
        var zebra = Product.Create("Zebra", "zebra", null, categoryId, ProductType.Simple);
        var apple = Product.Create("Apple", "apple", null, categoryId, ProductType.Simple);
        dbContext.Products.AddRange(zebra, apple);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetProductsListQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.First().Name.Should().Be("Apple");
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_ReturnsOnlyMatchingCategory()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var categoryA = Guid.NewGuid();
        var categoryB = Guid.NewGuid();
        var productA = Product.Create("Product A", "product-a", null, categoryA, ProductType.Simple);
        var productB = Product.Create("Product B", "product-b", null, categoryB, ProductType.Simple);
        dbContext.Products.AddRange(productA, productB);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetProductsListQuery(CategoryId: categoryA), CancellationToken.None);

        result.Items.Should().ContainSingle(i => i.Id == productA.Id);
    }

    [Fact]
    public async Task Handle_WithIsActiveFilter_ReturnsOnlyMatchingProducts()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var categoryId = Guid.NewGuid();
        var active = Product.Create("Active", "active", null, categoryId, ProductType.Simple);
        active.Activate();
        var inactive = Product.Create("Inactive", "inactive", null, categoryId, ProductType.Simple);
        dbContext.Products.AddRange(active, inactive);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetProductsListQuery(IsActive: true), CancellationToken.None);

        result.Items.Should().ContainSingle(i => i.Id == active.Id);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ReturnsMatchingProductsOnly()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var categoryId = Guid.NewGuid();
        var sneaker = Product.Create("Running Sneaker", "running-sneaker", null, categoryId, ProductType.Simple);
        var boot = Product.Create("Winter Boot", "winter-boot", null, categoryId, ProductType.Simple);
        dbContext.Products.AddRange(sneaker, boot);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetProductsListQuery(SearchTerm: "Sneaker"), CancellationToken.None);

        result.Items.Should().ContainSingle(i => i.Id == sneaker.Id);
    }

    [Fact]
    public async Task Handle_WithPrimaryImage_ReturnsPrimaryImageUrl()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        var image = product.AddImage("https://example.com/img.png", 0, true, null);
        dbContext.Products.Add(product);
        dbContext.ProductImages.Add(image);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetProductsListQuery(), CancellationToken.None);

        result.Items.Single().PrimaryImageUrl.Should().Be("https://example.com/img.png");
    }

    [Fact]
    public async Task Handle_WithPaging_ReturnsCorrectPage()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var categoryId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
            dbContext.Products.Add(Product.Create($"Product {i}", $"product-{i}", null, categoryId, ProductType.Simple));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetProductsListQuery(PageNumber: 2, PageSize: 2), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WithNoProducts_ReturnsEmptyResult()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new GetProductsListQueryHandler(dbContext);

        var result = await handler.Handle(new GetProductsListQuery(), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
