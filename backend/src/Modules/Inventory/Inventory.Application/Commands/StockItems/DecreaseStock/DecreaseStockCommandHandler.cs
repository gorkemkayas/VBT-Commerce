using Inventory.Application.Abstractions;
using Inventory.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Commands.StockItems.DecreaseStock;

public class DecreaseStockCommandHandler(IInventoryDbContext dbContext) : IRequestHandler<DecreaseStockCommand, Unit>
{
    public async Task<Unit> Handle(DecreaseStockCommand request, CancellationToken cancellationToken)
    {
        var stockItem = await dbContext.StockItems
            .FirstOrDefaultAsync(s => s.Id == request.StockItemId, cancellationToken)
            ?? throw new StockItemNotFoundException(request.StockItemId);

        stockItem.DecreaseQuantity(request.Quantity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
