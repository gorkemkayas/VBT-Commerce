using Inventory.Application.Abstractions;
using Inventory.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Commands.StockItems.IncreaseStock;

public class IncreaseStockCommandHandler(IInventoryDbContext dbContext) : IRequestHandler<IncreaseStockCommand, Unit>
{
    public async Task<Unit> Handle(IncreaseStockCommand request, CancellationToken cancellationToken)
    {
        var stockItem = await dbContext.StockItems
            .FirstOrDefaultAsync(s => s.Id == request.StockItemId, cancellationToken)
            ?? throw new StockItemNotFoundException(request.StockItemId);

        stockItem.IncreaseQuantity(request.Quantity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
