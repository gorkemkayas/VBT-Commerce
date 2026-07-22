namespace ECommerce.API.Controllers.Catalog.Requests;

public record UpdateCategoryRequest(string Name, string Slug, string? Description, int DisplayOrder, Guid? ParentCategoryId);
