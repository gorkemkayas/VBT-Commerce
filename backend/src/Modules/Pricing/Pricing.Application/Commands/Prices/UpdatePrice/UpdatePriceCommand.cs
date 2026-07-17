using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Pricing.Application.Commands.Prices.UpdatePrice;

public record UpdatePriceCommand(Guid PriceId, decimal Amount) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
