namespace ECommerce.API.Controllers.Catalog;

public record AddProductAttributeRequest(string Name, string Value, int DisplayOrder);

public record UpdateProductAttributeRequest(string Name, string Value, int DisplayOrder);
