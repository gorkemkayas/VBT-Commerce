namespace ECommerce.API.Controllers.Catalog;

public record AddProductImageRequest(string Url, int DisplayOrder, bool IsPrimary, Guid? ProductVariantId);
