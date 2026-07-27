using Catalog.Application.Queries.Products.GetProductBySlug;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Queries.Products;

public class GetProductBySlugQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_ReturnsDto()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", "desc", Guid.NewGuid(), ProductType.Simple);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductBySlugQueryHandler(dbContext);

        var dto = await handler.Handle(new GetProductBySlugQuery("SHIRT"), CancellationToken.None);

        dto.Id.Should().Be(product.Id);
        dto.Slug.Should().Be("shirt");
    }

    [Fact]
    public async Task Handle_WithNonExistentSlug_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new GetProductBySlugQueryHandler(dbContext);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(new GetProductBySlugQuery("missing"), CancellationToken.None));
    }
}
