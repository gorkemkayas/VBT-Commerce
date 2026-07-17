using MediatR;
using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;
using Pricing.Application.Integrations;
using Pricing.Domain.Entities;
using Pricing.Domain.Exceptions;

namespace Pricing.Application.Commands.Prices.CreatePrice;

public class CreatePriceCommandHandler(
    IPricingDbContext dbContext,
    ICatalogIntegrationService catalogIntegrationService) : IRequestHandler<CreatePriceCommand, Guid>
{
    public async Task<Guid> Handle(CreatePriceCommand request, CancellationToken cancellationToken)
    {
        var sellableItemExists = await catalogIntegrationService.SellableItemExistsAsync(
            request.SellableItemId, request.SellableItemType, cancellationToken);

        if (!sellableItemExists)
            throw new PricingSellableItemNotFoundException(request.SellableItemId, request.SellableItemType);

        var alreadyExists = await dbContext.Prices.AnyAsync(
            p => p.SellableItemId == request.SellableItemId && p.SellableItemType == request.SellableItemType,
            cancellationToken);

        if (alreadyExists)
            throw new PriceAlreadyExistsException(request.SellableItemId, request.SellableItemType);

        var price = Price.Create(request.SellableItemId, request.SellableItemType, request.Amount);

        dbContext.Prices.Add(price);
        await dbContext.SaveChangesAsync(cancellationToken);

        return price.Id;
    }
}
