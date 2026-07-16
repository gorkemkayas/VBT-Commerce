using Catalog.Domain.Enums;
using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

/// <summary>
/// Aggregate root for the catalog's product identity: name/category/type plus its descriptive
/// attributes, variant axes, variants and images. Price lives in the Pricing module, stock in the
/// Inventory module — neither is tracked here.
/// </summary>
public class Product
{
    private readonly List<ProductAttribute> _attributes = [];
    private readonly List<ProductVariantAttribute> _variantAttributes = [];
    private readonly List<ProductVariant> _variants = [];
    private readonly List<ProductImage> _images = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public ProductType ProductType { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();
    public IReadOnlyCollection<ProductVariantAttribute> VariantAttributes => _variantAttributes.AsReadOnly();
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private Product()
    {
    }

    public static Product Create(string name, string slug, string? description, Guid categoryId, ProductType productType)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Slug = NormalizeSlug(slug),
            Description = description,
            CategoryId = categoryId,
            ProductType = productType,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string slug, string? description, Guid categoryId)
    {
        Name = name.Trim();
        Slug = NormalizeSlug(slug);
        Description = description;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (ProductType == ProductType.Variant && !_variants.Any(v => v.IsActive))
            throw new ProductMustHaveAtLeastOneVariantException(Id);

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Also used as the soft-delete operation for a product.</summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // ---- Descriptive attributes ----

    public ProductAttribute AddAttribute(string name, string value, int displayOrder)
    {
        var attribute = ProductAttribute.Create(Id, name, value, displayOrder);
        _attributes.Add(attribute);
        UpdatedAt = DateTime.UtcNow;
        return attribute;
    }

    public void UpdateAttribute(Guid attributeId, string name, string value, int displayOrder)
    {
        var attribute = _attributes.FirstOrDefault(a => a.Id == attributeId)
            ?? throw new ProductAttributeNotFoundException(attributeId);

        attribute.Update(name, value, displayOrder);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAttribute(Guid attributeId)
    {
        var attribute = _attributes.FirstOrDefault(a => a.Id == attributeId)
            ?? throw new ProductAttributeNotFoundException(attributeId);

        _attributes.Remove(attribute);
        UpdatedAt = DateTime.UtcNow;
    }

    // ---- Variant axes ----

    public ProductVariantAttribute AddVariantAttribute(string name, int displayOrder)
    {
        EnsureIsVariantType();

        var variantAttribute = ProductVariantAttribute.Create(Id, name, displayOrder);
        _variantAttributes.Add(variantAttribute);
        UpdatedAt = DateTime.UtcNow;
        return variantAttribute;
    }

    public void RemoveVariantAttribute(Guid variantAttributeId)
    {
        var variantAttribute = _variantAttributes.FirstOrDefault(a => a.Id == variantAttributeId)
            ?? throw new ProductVariantAttributeNotFoundException(variantAttributeId);

        var isInUse = _variants.Any(v => v.OptionValues.Any(ov => ov.ProductVariantAttributeId == variantAttributeId));
        if (isInUse)
            throw new VariantAttributeInUseException(variantAttributeId);

        _variantAttributes.Remove(variantAttribute);
        UpdatedAt = DateTime.UtcNow;
    }

    // ---- Variants ----

    public ProductVariant AddVariant(string sku, IReadOnlyDictionary<Guid, string> optionValuesByAttributeId)
    {
        EnsureIsVariantType();
        EnsureOptionValuesMatchVariantAttributes(optionValuesByAttributeId);
        EnsureSkuIsUnique(sku, excludingVariantId: null);
        EnsureOptionCombinationIsUnique(optionValuesByAttributeId, excludingVariantId: null);

        var variant = ProductVariant.Create(Id, sku, optionValuesByAttributeId);
        _variants.Add(variant);
        UpdatedAt = DateTime.UtcNow;
        return variant;
    }

    public void UpdateVariant(Guid variantId, string sku, IReadOnlyDictionary<Guid, string> optionValuesByAttributeId, bool isActive)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId)
            ?? throw new ProductVariantNotFoundException(variantId);

        EnsureOptionValuesMatchVariantAttributes(optionValuesByAttributeId);
        EnsureSkuIsUnique(sku, excludingVariantId: variantId);
        EnsureOptionCombinationIsUnique(optionValuesByAttributeId, excludingVariantId: variantId);

        if (IsActive && !isActive && ProductType == ProductType.Variant
            && _variants.Count(v => v.IsActive) == 1 && variant.IsActive)
        {
            throw new ProductMustHaveAtLeastOneVariantException(Id);
        }

        variant.UpdateSku(sku);
        variant.ReplaceOptionValues(optionValuesByAttributeId);

        if (isActive) variant.Activate();
        else variant.Deactivate();

        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveVariant(Guid variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId)
            ?? throw new ProductVariantNotFoundException(variantId);

        if (IsActive && ProductType == ProductType.Variant
            && variant.IsActive && _variants.Count(v => v.IsActive) == 1)
        {
            throw new ProductMustHaveAtLeastOneVariantException(Id);
        }

        _variants.Remove(variant);
        UpdatedAt = DateTime.UtcNow;
    }

    // ---- Images ----

    public ProductImage AddImage(string url, int displayOrder, bool isPrimary, Guid? productVariantId)
    {
        if (productVariantId is not null && _variants.All(v => v.Id != productVariantId))
            throw new ProductVariantNotFoundException(productVariantId.Value);

        if (isPrimary)
            UnsetExistingPrimaryImage();

        var image = ProductImage.Create(Id, productVariantId, url, displayOrder, isPrimary);
        _images.Add(image);
        UpdatedAt = DateTime.UtcNow;
        return image;
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new ProductImageNotFoundException(imageId);

        _images.Remove(image);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPrimaryImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new ProductImageNotFoundException(imageId);

        UnsetExistingPrimaryImage();
        image.SetPrimary(true);
        UpdatedAt = DateTime.UtcNow;
    }

    private void UnsetExistingPrimaryImage()
    {
        foreach (var existingPrimary in _images.Where(i => i.IsPrimary))
            existingPrimary.SetPrimary(false);
    }

    // ---- Invariant helpers ----

    private void EnsureIsVariantType()
    {
        if (ProductType != ProductType.Variant)
            throw new InvalidProductTypeOperationException(
                $"Product '{Id}' is of type 'Simple' and cannot define variant attributes or variants.");
    }

    private void EnsureOptionValuesMatchVariantAttributes(IReadOnlyDictionary<Guid, string> optionValuesByAttributeId)
    {
        var definedAttributeIds = _variantAttributes.Select(a => a.Id).ToHashSet();
        var suppliedAttributeIds = optionValuesByAttributeId.Keys.ToHashSet();

        if (!definedAttributeIds.SetEquals(suppliedAttributeIds))
            throw new VariantAttributeMismatchException(Id);
    }

    private void EnsureSkuIsUnique(string sku, Guid? excludingVariantId)
    {
        var normalizedSku = sku.Trim();
        var isDuplicate = _variants
            .Where(v => v.Id != excludingVariantId)
            .Any(v => string.Equals(v.Sku, normalizedSku, StringComparison.OrdinalIgnoreCase));

        if (isDuplicate)
            throw new DuplicateSkuException(sku);
    }

    private void EnsureOptionCombinationIsUnique(IReadOnlyDictionary<Guid, string> optionValuesByAttributeId, Guid? excludingVariantId)
    {
        var candidateCombination = optionValuesByAttributeId
            .Select(kv => (AttributeId: kv.Key, Value: kv.Value.Trim().ToLowerInvariant()))
            .ToHashSet();

        var isDuplicate = _variants
            .Where(v => v.Id != excludingVariantId)
            .Any(v => v.OptionCombination().SetEquals(candidateCombination));

        if (isDuplicate)
            throw new DuplicateVariantOptionCombinationException(Id);
    }

    private static string NormalizeSlug(string slug) => slug.Trim().ToLowerInvariant();
}
