using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Pricing.Domain.Enums;

namespace Pricing.Application.Commands.Prices.CreatePrice;

public record CreatePriceCommand(Guid SellableItemId, PriceItemType SellableItemType, decimal Amount) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
