using Inventory.Application.Abstractions;
using Inventory.Application.Integrations;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Commands.StockItems.CreateStockItem;

public class CreateStockItemCommandHandler(
    IInventoryDbContext dbContext,
    ICatalogIntegrationService catalogIntegrationService) : IRequestHandler<CreateStockItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateStockItemCommand request, CancellationToken cancellationToken)
    {
        var sellableItemExists = await catalogIntegrationService.SellableItemExistsAsync(
            request.SellableItemId, request.SellableItemType, cancellationToken);

        if (!sellableItemExists)
            throw new SellableItemNotFoundException(request.SellableItemId);

        var alreadyExists = await dbContext.StockItems
            .AnyAsync(s => s.SellableItemId == request.SellableItemId && s.SellableItemType == request.SellableItemType, cancellationToken);

        if (alreadyExists)
            throw new DuplicateStockItemException(request.SellableItemId);

        var stockItem = StockItem.Create(request.SellableItemId, request.SellableItemType, request.InitialQuantity);

        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return stockItem.Id;
    }
}
