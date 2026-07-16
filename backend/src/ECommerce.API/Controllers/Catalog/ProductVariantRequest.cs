namespace ECommerce.API.Controllers.Catalog;

public record AddProductVariantRequest(string Sku, IReadOnlyDictionary<Guid, string> OptionValues);

public record UpdateProductVariantRequest(string Sku, IReadOnlyDictionary<Guid, string> OptionValues, bool IsActive);
