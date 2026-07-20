using MediatR;
using Pricing.Application.Services;
using Pricing.Contracts;

namespace Pricing.Application.Queries.Calculate.CalculateCustomerOrderPrice;

public class CalculateCustomerOrderPriceQueryHandler(PriceCalculationService priceCalculationService)
    : IRequestHandler<CalculateCustomerOrderPriceQuery, PriceCalculationResultDto>
{
    public Task<PriceCalculationResultDto> Handle(CalculateCustomerOrderPriceQuery request, CancellationToken cancellationToken)
    {
        var owner = PricingOwnerKey.ForCustomer(request.CustomerId);
        return priceCalculationService.CalculateAsync(owner, request.Items, request.CouponCodes, cancellationToken);
    }
}
