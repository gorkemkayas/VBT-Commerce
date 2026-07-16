namespace Catalog.Domain.Entities;

/// <summary>
/// A sellable variation of a product (e.g. "Red / M"). Carries only catalog identity (SKU) —
/// price lives in the Pricing module, stock in the Inventory module.
/// </summary>
public class ProductVariant
{
    private readonly List<ProductVariantOptionValue> _optionValues = [];

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Sku { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<ProductVariantOptionValue> OptionValues => _optionValues.AsReadOnly();

    private ProductVariant()
    {
    }

    internal static ProductVariant Create(Guid productId, string sku, IReadOnlyDictionary<Guid, string> optionValuesByAttributeId)
    {
        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Sku = NormalizeSku(sku),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var (attributeId, value) in optionValuesByAttributeId)
            variant._optionValues.Add(ProductVariantOptionValue.Create(variant.Id, attributeId, value));

        return variant;
    }

    internal void UpdateSku(string sku)
    {
        Sku = NormalizeSku(sku);
        UpdatedAt = DateTime.UtcNow;
    }

    internal void ReplaceOptionValues(IReadOnlyDictionary<Guid, string> optionValuesByAttributeId)
    {
        _optionValues.Clear();
        foreach (var (attributeId, value) in optionValuesByAttributeId)
            _optionValues.Add(ProductVariantOptionValue.Create(Id, attributeId, value));
        UpdatedAt = DateTime.UtcNow;
    }

    internal void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Set of (attributeId, value) pairs identifying this variant's option combination.</summary>
    internal HashSet<(Guid AttributeId, string Value)> OptionCombination()
        => _optionValues.Select(v => (v.ProductVariantAttributeId, v.Value.ToLowerInvariant())).ToHashSet();

    private static string NormalizeSku(string sku) => sku.Trim().ToUpperInvariant();
}
