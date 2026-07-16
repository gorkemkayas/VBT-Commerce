namespace Catalog.Domain.Entities;

/// <summary>
/// A free-form descriptive key/value fact about a product (e.g. "Material" -> "Cotton").
/// Purely informational — not used to define variants (see <see cref="ProductVariantAttribute"/>).
/// </summary>
public class ProductAttribute
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Value { get; private set; } = null!;
    public int DisplayOrder { get; private set; }

    private ProductAttribute()
    {
    }

    internal static ProductAttribute Create(Guid productId, string name, string value, int displayOrder)
    {
        return new ProductAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = name.Trim(),
            Value = value.Trim(),
            DisplayOrder = displayOrder
        };
    }

    internal void Update(string name, string value, int displayOrder)
    {
        Name = name.Trim();
        Value = value.Trim();
        DisplayOrder = displayOrder;
    }
}
