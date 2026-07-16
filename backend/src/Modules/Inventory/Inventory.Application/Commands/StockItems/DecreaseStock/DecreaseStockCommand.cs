using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Inventory.Application.Commands.StockItems.DecreaseStock;

public record DecreaseStockCommand(Guid StockItemId, int Quantity) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
