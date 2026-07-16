using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Inventory.Application.Commands.StockItems.IncreaseStock;

public record IncreaseStockCommand(Guid StockItemId, int Quantity) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
