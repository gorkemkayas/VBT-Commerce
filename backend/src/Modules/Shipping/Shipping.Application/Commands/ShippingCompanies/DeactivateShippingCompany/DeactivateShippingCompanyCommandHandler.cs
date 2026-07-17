using MediatR;
using Shipping.Application.Abstractions;
using Shipping.Domain.Exceptions;

namespace Shipping.Application.Commands.ShippingCompanies.DeactivateShippingCompany;

public class DeactivateShippingCompanyCommandHandler(IShippingDbContext dbContext) : IRequestHandler<DeactivateShippingCompanyCommand, Unit>
{
    public async Task<Unit> Handle(DeactivateShippingCompanyCommand request, CancellationToken cancellationToken)
    {
        var shippingCompany = await dbContext.ShippingCompanies.FindAsync([request.ShippingCompanyId], cancellationToken)
            ?? throw new ShippingCompanyNotFoundException(request.ShippingCompanyId);

        shippingCompany.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
