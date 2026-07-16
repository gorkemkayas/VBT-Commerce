namespace ECommerce.API.Controllers.Catalog;

public record UpdateCategoryRequest(string Name, string Slug, string? Description, int DisplayOrder, Guid? ParentCategoryId);
