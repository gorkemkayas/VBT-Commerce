using MediatR;
using Pricing.Application.Abstractions;
using Pricing.Domain.Exceptions;

namespace Pricing.Application.Commands.Prices.UpdatePrice;

public class UpdatePriceCommandHandler(IPricingDbContext dbContext) : IRequestHandler<UpdatePriceCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePriceCommand request, CancellationToken cancellationToken)
    {
        var price = await dbContext.Prices.FindAsync([request.PriceId], cancellationToken)
            ?? throw new PriceNotFoundException(request.PriceId);

        price.UpdateAmount(request.Amount);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
