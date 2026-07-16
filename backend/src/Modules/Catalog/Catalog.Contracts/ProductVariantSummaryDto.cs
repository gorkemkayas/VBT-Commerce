namespace Catalog.Contracts;

public record ProductVariantSummaryDto(Guid Id, Guid ProductId, string Sku, bool IsActive);
