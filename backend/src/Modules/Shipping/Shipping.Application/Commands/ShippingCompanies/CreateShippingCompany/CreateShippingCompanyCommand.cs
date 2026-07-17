using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;

namespace Shipping.Application.Commands.ShippingCompanies.CreateShippingCompany;

public record CreateShippingCompanyCommand(string Name, decimal Fee) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
