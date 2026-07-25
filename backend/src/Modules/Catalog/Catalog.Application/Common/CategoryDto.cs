namespace Catalog.Application.Common;

public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    Guid? ParentCategoryId,
    int DisplayOrder,
    bool IsActive);

public record CategoryTreeDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    bool IsActive,
    IReadOnlyCollection<CategoryTreeDto> Children);
