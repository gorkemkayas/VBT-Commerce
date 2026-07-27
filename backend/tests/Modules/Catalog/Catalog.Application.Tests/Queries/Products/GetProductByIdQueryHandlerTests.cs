using Catalog.Application.Queries.Products.GetProductById;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.Application.Tests.Queries.Products;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_ReturnsDto()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var product = Product.Create("Shirt", "shirt", "desc", Guid.NewGuid(), ProductType.Variant);
        var attribute = product.AddAttribute("Material", "Cotton", 0);
        var colorAttribute = product.AddVariantAttribute("Color", 0);
        var variant = product.AddVariant("SKU-1", new Dictionary<Guid, string> { [colorAttribute.Id] = "Red" });
        var image = product.AddImage("https://example.com/img.png", 0, true, null);
        dbContext.Products.Add(product);
        dbContext.ProductAttributes.Add(attribute);
        dbContext.ProductVariantAttributes.Add(colorAttribute);
        dbContext.ProductVariants.Add(variant);
        dbContext.ProductVariantOptionValues.AddRange(variant.OptionValues);
        dbContext.ProductImages.Add(image);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetProductByIdQueryHandler(dbContext);

        var dto = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        dto.Id.Should().Be(product.Id);
        dto.Name.Should().Be("Shirt");
        dto.Attributes.Should().ContainSingle(a => a.Name == "Material" && a.Value == "Cotton");
        dto.VariantAttributes.Should().ContainSingle(a => a.Name == "Color");
        dto.Variants.Should().ContainSingle(v => v.Sku == "SKU-1");
        dto.Variants.Single().OptionValues.Should().ContainSingle(ov => ov.Value == "Red" && ov.AttributeName == "Color");
        dto.Images.Should().ContainSingle(i => i.Url == "https://example.com/img.png" && i.IsPrimary);
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsProductNotFoundException()
    {
        using var dbContext = TestCatalogDbContextFactory.Create();
        var handler = new GetProductByIdQueryHandler(dbContext);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
