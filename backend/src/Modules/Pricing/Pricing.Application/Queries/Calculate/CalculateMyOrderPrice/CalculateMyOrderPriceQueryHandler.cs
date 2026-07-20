using BuildingBlocks.Application.Security;
using MediatR;
using Pricing.Application.Services;
using Pricing.Contracts;

namespace Pricing.Application.Queries.Calculate.CalculateMyOrderPrice;

public class CalculateMyOrderPriceQueryHandler(PriceCalculationService priceCalculationService, ICurrentUserService currentUserService)
    : IRequestHandler<CalculateMyOrderPriceQuery, PriceCalculationResultDto>
{
    public Task<PriceCalculationResultDto> Handle(CalculateMyOrderPriceQuery request, CancellationToken cancellationToken)
    {
        var owner = PricingOwnerKey.ForCustomer(currentUserService.UserId!.Value);
        return priceCalculationService.CalculateAsync(owner, request.Items, request.CouponCodes, cancellationToken);
    }
}
