using MediatR;
using Microsoft.EntityFrameworkCore;
using Shipping.Application.Abstractions;
using Shipping.Domain.Entities;
using Shipping.Domain.Exceptions;

namespace Shipping.Application.Commands.ShippingCompanies.CreateShippingCompany;

public class CreateShippingCompanyCommandHandler(IShippingDbContext dbContext) : IRequestHandler<CreateShippingCompanyCommand, Guid>
{
    public async Task<Guid> Handle(CreateShippingCompanyCommand request, CancellationToken cancellationToken)
    {
        var alreadyExists = await dbContext.ShippingCompanies.AnyAsync(
            c => c.Name == request.Name, cancellationToken);

        if (alreadyExists)
            throw new ShippingCompanyAlreadyExistsException(request.Name);

        var shippingCompany = ShippingCompany.Create(request.Name, request.Fee);

        dbContext.ShippingCompanies.Add(shippingCompany);
        await dbContext.SaveChangesAsync(cancellationToken);

        return shippingCompany.Id;
    }
}
