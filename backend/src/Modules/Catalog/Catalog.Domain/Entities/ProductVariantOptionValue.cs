namespace Catalog.Domain.Entities;

/// <summary>
/// The value a variant carries for one of its product's variant attributes (e.g. axis "Color" -> "Red").
/// </summary>
public class ProductVariantOptionValue
{
    public Guid Id { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public Guid ProductVariantAttributeId { get; private set; }
    public string Value { get; private set; } = null!;

    private ProductVariantOptionValue()
    {
    }

    internal static ProductVariantOptionValue Create(Guid productVariantId, Guid productVariantAttributeId, string value)
    {
        return new ProductVariantOptionValue
        {
            Id = Guid.NewGuid(),
            ProductVariantId = productVariantId,
            ProductVariantAttributeId = productVariantAttributeId,
            Value = value.Trim()
        };
    }
}
