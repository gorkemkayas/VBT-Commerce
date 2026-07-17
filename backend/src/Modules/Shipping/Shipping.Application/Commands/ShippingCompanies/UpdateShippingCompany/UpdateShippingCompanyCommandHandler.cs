using MediatR;
using Shipping.Application.Abstractions;
using Shipping.Domain.Exceptions;

namespace Shipping.Application.Commands.ShippingCompanies.UpdateShippingCompany;

public class UpdateShippingCompanyCommandHandler(IShippingDbContext dbContext) : IRequestHandler<UpdateShippingCompanyCommand, Unit>
{
    public async Task<Unit> Handle(UpdateShippingCompanyCommand request, CancellationToken cancellationToken)
    {
        var shippingCompany = await dbContext.ShippingCompanies.FindAsync([request.ShippingCompanyId], cancellationToken)
            ?? throw new ShippingCompanyNotFoundException(request.ShippingCompanyId);

        shippingCompany.Update(request.Name, request.Fee);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
