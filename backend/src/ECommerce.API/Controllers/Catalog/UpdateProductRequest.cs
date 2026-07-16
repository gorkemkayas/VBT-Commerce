namespace ECommerce.API.Controllers.Catalog;

public record UpdateProductRequest(string Name, string Slug, string? Description, Guid CategoryId);
