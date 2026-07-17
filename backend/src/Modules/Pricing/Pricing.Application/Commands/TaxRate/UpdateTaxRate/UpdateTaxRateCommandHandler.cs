using MediatR;
using Microsoft.EntityFrameworkCore;
using Pricing.Application.Abstractions;

namespace Pricing.Application.Commands.TaxRate.UpdateTaxRate;

public class UpdateTaxRateCommandHandler(IPricingDbContext dbContext) : IRequestHandler<UpdateTaxRateCommand, Unit>
{
    public async Task<Unit> Handle(UpdateTaxRateCommand request, CancellationToken cancellationToken)
    {
        var taxRate = await dbContext.TaxRates.FirstAsync(t => t.Id == Pricing.Domain.Entities.TaxRate.SingletonId, cancellationToken);

        taxRate.UpdateRate(request.Rate);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
