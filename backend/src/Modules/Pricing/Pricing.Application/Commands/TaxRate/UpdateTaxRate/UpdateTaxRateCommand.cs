using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Pricing.Application.Commands.TaxRate.UpdateTaxRate;

public record UpdateTaxRateCommand(decimal Rate) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
