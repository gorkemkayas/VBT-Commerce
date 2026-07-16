namespace Catalog.Domain.Entities;

/// <summary>
/// Defines a dynamic variant axis scoped to a single product (e.g. "Color", "Strap Type").
/// Each <see cref="ProductVariant"/> of the product must supply exactly one value
/// (<see cref="ProductVariantOptionValue"/>) per axis defined here.
/// </summary>
public class ProductVariantAttribute
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = null!;
    public int DisplayOrder { get; private set; }

    private ProductVariantAttribute()
    {
    }

    internal static ProductVariantAttribute Create(Guid productId, string name, int displayOrder)
    {
        return new ProductVariantAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = name.Trim(),
            DisplayOrder = displayOrder
        };
    }

    internal void Rename(string name, int displayOrder)
    {
        Name = name.Trim();
        DisplayOrder = displayOrder;
    }
}
