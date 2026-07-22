namespace ECommerce.API.Controllers.Shipping.Requests;

public record CreateShippingCompanyRequest(string Name, decimal Fee);

public record UpdateShippingCompanyRequest(string Name, decimal Fee);
