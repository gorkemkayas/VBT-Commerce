using Catalog.Application.Commands.Products.AddProductAttribute;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Commands.Products;

public class AddProductAttributeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_AddsAttribute()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Cap", "cap", null, Guid.NewGuid(), ProductType.Simple);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new AddProductAttributeCommandHandler(dbContext);
        var command = new AddProductAttributeCommand(product.Id, "Material", "Cotton", 0);

        var attributeId = await handler.Handle(command, CancellationToken.None);

        attributeId.Should().NotBe(Guid.Empty);
        var stored = await dbContext.ProductAttributes.FindAsync(attributeId);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Material");
        stored.Value.Should().Be("Cotton");
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new AddProductAttributeCommandHandler(dbContext);
        var command = new AddProductAttributeCommand(Guid.NewGuid(), "Material", "Cotton", 0);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
