using Shipping.Domain.Entities;

namespace Shipping.Application.Common;

public static class ShippingCompanyMapper
{
    public static ShippingCompanyDto ToDto(ShippingCompany shippingCompany)
        => new(shippingCompany.Id, shippingCompany.Name, shippingCompany.Fee, shippingCompany.IsActive);
}
