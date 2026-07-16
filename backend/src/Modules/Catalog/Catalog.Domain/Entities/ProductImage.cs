namespace Catalog.Domain.Entities;

public class ProductImage
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid? ProductVariantId { get; private set; }
    public string Url { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public bool IsPrimary { get; private set; }

    private ProductImage()
    {
    }

    internal static ProductImage Create(Guid productId, Guid? productVariantId, string url, int displayOrder, bool isPrimary)
    {
        return new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ProductVariantId = productVariantId,
            Url = url.Trim(),
            DisplayOrder = displayOrder,
            IsPrimary = isPrimary
        };
    }

    internal void SetPrimary(bool isPrimary) => IsPrimary = isPrimary;
}
