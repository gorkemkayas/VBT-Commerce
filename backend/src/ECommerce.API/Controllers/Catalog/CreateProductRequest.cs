using Catalog.Domain.Enums;

namespace ECommerce.API.Controllers.Catalog;

public record CreateProductRequest(string Name, string Slug, string? Description, Guid CategoryId, ProductType ProductType);
