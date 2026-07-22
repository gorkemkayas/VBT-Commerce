using Catalog.Domain.Enums;

namespace ECommerce.API.Controllers.Catalog.Requests;

public record CreateProductRequest(string Name, string Slug, string? Description, Guid CategoryId, ProductType ProductType);
