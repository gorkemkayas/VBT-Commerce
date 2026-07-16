namespace Catalog.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Category()
    {
    }

    public static Category Create(string name, string slug, string? description, Guid? parentCategoryId, int displayOrder)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Slug = NormalizeSlug(slug),
            Description = description,
            ParentCategoryId = parentCategoryId,
            DisplayOrder = displayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string slug, string? description, int displayOrder)
    {
        Name = name.Trim();
        Slug = NormalizeSlug(slug);
        Description = description;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeParent(Guid? parentCategoryId)
    {
        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string NormalizeSlug(string slug) => slug.Trim().ToLowerInvariant();
}
