namespace ECommerce.API.Controllers.Catalog.Requests;

public record AddProductImageRequest(string Url, int DisplayOrder, bool IsPrimary, Guid? ProductVariantId);
