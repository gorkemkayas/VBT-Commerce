using MediatR;
using Pricing.Application.Common;
using Pricing.Application.Services;

namespace Pricing.Application.Queries.Calculate.CalculateGuestOrderPrice;

public class CalculateGuestOrderPriceQueryHandler(PriceCalculationService priceCalculationService)
    : IRequestHandler<CalculateGuestOrderPriceQuery, PriceCalculationResultDto>
{
    public Task<PriceCalculationResultDto> Handle(CalculateGuestOrderPriceQuery request, CancellationToken cancellationToken)
    {
        var owner = PricingOwnerKey.ForGuestCustomer(request.GuestCustomerId);
        return priceCalculationService.CalculateAsync(owner, request.Items, request.CouponCodes, cancellationToken);
    }
}
