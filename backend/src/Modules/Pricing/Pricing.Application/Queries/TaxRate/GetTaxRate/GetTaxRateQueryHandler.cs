using MediatR;
using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;

namespace Pricing.Application.Queries.TaxRate.GetTaxRate;

public class GetTaxRateQueryHandler(IPricingDbContext dbContext) : IRequestHandler<GetTaxRateQuery, decimal>
{
    public async Task<decimal> Handle(GetTaxRateQuery request, CancellationToken cancellationToken)
    {
        var taxRate = await dbContext.TaxRates.FirstAsync(t => t.Id == Pricing.Domain.Entities.TaxRate.SingletonId, cancellationToken);
        return taxRate.Rate;
    }
}
