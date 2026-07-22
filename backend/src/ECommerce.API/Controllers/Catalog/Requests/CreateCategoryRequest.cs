namespace ECommerce.API.Controllers.Catalog.Requests;

public record CreateCategoryRequest(string Name, string Slug, string? Description, Guid? ParentCategoryId, int DisplayOrder);
