namespace ECommerce.API.Controllers.Catalog.Requests;

public record UpdateCategoryRequest(string Name, string Slug, string? Description, string? ImageUrl, int DisplayOrder, Guid? ParentCategoryId);
