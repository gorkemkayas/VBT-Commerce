namespace ECommerce.API.Controllers.Shipping;

public record CreateShippingCompanyRequest(string Name, decimal Fee);

public record UpdateShippingCompanyRequest(string Name, decimal Fee);
