using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Shipping.Application.Common;

namespace Shipping.Application.Queries.ShippingCompanies.GetShippingCompaniesList;

public record GetShippingCompaniesListQuery(int PageNumber = 1, int PageSize = 20) : IQuery<PagedResult<ShippingCompanyDto>>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
