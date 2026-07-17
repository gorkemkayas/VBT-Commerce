using BuildingBlocks.Application.Messaging;
using Shipping.Application.Common;

namespace Shipping.Application.Queries.ShippingCompanies.GetActiveShippingCompanies;

public record GetActiveShippingCompaniesQuery : IQuery<IReadOnlyList<ShippingCompanyDto>>;
