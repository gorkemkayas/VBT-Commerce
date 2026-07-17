using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Shipping.Application.Commands.ShippingCompanies.DeactivateShippingCompany;

public record DeactivateShippingCompanyCommand(Guid ShippingCompanyId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
