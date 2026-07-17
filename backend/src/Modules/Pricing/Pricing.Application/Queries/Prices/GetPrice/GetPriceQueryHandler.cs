using MediatR;
using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;
using Pricing.Application.Common;
using Pricing.Domain.Exceptions;

namespace Pricing.Application.Queries.Prices.GetPrice;

public class GetPriceQueryHandler(IPricingDbContext dbContext) : IRequestHandler<GetPriceQuery, PriceDto>
{
    public async Task<PriceDto> Handle(GetPriceQuery request, CancellationToken cancellationToken)
    {
        var price = await dbContext.Prices.FirstOrDefaultAsync(
            p => p.SellableItemId == request.SellableItemId && p.SellableItemType == request.SellableItemType,
            cancellationToken)
            ?? throw new PriceNotFoundException(request.SellableItemId, request.SellableItemType);

        return PriceMapper.ToDto(price);
    }
}
