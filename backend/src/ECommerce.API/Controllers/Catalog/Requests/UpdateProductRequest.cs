namespace ECommerce.API.Controllers.Catalog.Requests;

public record UpdateProductRequest(string Name, string Slug, string? Description, Guid CategoryId);
