namespace Shipping.Application.Common;

public record ShippingCompanyDto(Guid Id, string Name, decimal Fee, bool IsActive);
