using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Shipping.Application.Commands.ShippingCompanies.UpdateShippingCompany;

public record UpdateShippingCompanyCommand(Guid ShippingCompanyId, string Name, decimal Fee) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
