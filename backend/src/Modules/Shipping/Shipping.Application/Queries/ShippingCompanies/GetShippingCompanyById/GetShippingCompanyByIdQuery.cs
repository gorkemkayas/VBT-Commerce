using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Shipping.Application.Common;

namespace Shipping.Application.Queries.ShippingCompanies.GetShippingCompanyById;

public record GetShippingCompanyByIdQuery(Guid ShippingCompanyId) : IQuery<ShippingCompanyDto>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
