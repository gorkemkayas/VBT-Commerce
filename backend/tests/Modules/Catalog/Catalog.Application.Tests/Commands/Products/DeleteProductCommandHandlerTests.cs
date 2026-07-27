using Catalog.Application.Commands.Products.DeleteProduct;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_DeactivatesProduct()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        product.Activate();
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteProductCommandHandler(dbContext);

        await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        var stored = await dbContext.Products.FindAsync(product.Id);
        stored!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new DeleteProductCommandHandler(dbContext);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(new DeleteProductCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
