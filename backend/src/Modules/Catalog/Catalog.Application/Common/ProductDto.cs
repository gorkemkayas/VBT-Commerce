using Catalog.Domain.Enums;

namespace Catalog.Application.Common;

public record ProductAttributeDto(Guid Id, string Name, string Value, int DisplayOrder);

public record ProductVariantAttributeDto(Guid Id, string Name, int DisplayOrder);

public record ProductVariantOptionValueDto(Guid ProductVariantAttributeId, string AttributeName, string Value);

public record ProductVariantDto(Guid Id, string Sku, bool IsActive, IReadOnlyCollection<ProductVariantOptionValueDto> OptionValues);

public record ProductImageDto(Guid Id, Guid? ProductVariantId, string Url, int DisplayOrder, bool IsPrimary);

public record ProductDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    Guid CategoryId,
    ProductType ProductType,
    bool IsActive,
    IReadOnlyCollection<ProductAttributeDto> Attributes,
    IReadOnlyCollection<ProductVariantAttributeDto> VariantAttributes,
    IReadOnlyCollection<ProductVariantDto> Variants,
    IReadOnlyCollection<ProductImageDto> Images);

public record ProductListItemDto(
    Guid Id,
    string Name,
    string Slug,
    Guid CategoryId,
    ProductType ProductType,
    bool IsActive,
    string? PrimaryImageUrl);
