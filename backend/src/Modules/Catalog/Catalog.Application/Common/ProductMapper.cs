using Catalog.Domain.Entities;

namespace Catalog.Application.Common;

public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        var variantAttributesById = product.VariantAttributes.ToDictionary(a => a.Id, a => a.Name);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Slug,
            product.Description,
            product.CategoryId,
            product.ProductType,
            product.IsActive,
            product.Attributes
                .Select(a => new ProductAttributeDto(a.Id, a.Name, a.Value, a.DisplayOrder))
                .ToList(),
            product.VariantAttributes
                .Select(a => new ProductVariantAttributeDto(a.Id, a.Name, a.DisplayOrder))
                .ToList(),
            product.Variants
                .Select(v => new ProductVariantDto(
                    v.Id,
                    v.Sku,
                    v.IsActive,
                    v.OptionValues
                        .Select(ov => new ProductVariantOptionValueDto(
                            ov.ProductVariantAttributeId,
                            variantAttributesById.GetValueOrDefault(ov.ProductVariantAttributeId, string.Empty),
                            ov.Value))
                        .ToList()))
                .ToList(),
            product.Images
                .Select(i => new ProductImageDto(i.Id, i.ProductVariantId, i.Url, i.DisplayOrder, i.IsPrimary))
                .ToList());
    }
}
