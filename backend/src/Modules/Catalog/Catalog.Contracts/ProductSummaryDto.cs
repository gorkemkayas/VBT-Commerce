using Catalog.Domain.Enums;

namespace Catalog.Contracts;

public record ProductSummaryDto(Guid Id, string Name, string Slug, ProductType ProductType, bool IsActive, Guid CategoryId);
