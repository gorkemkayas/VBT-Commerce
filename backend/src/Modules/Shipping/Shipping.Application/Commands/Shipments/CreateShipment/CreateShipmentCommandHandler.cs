using MediatR;
using Shipping.Application.Abstractions;
using Shipping.Domain.Entities;
using Shipping.Domain.Exceptions;

namespace Shipping.Application.Commands.Shipments.CreateShipment;

public class CreateShipmentCommandHandler(IShippingDbContext dbContext) : IRequestHandler<CreateShipmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        var shippingCompany = await dbContext.ShippingCompanies.FindAsync([request.ShippingCompanyId], cancellationToken)
            ?? throw new ShippingCompanyNotFoundException(request.ShippingCompanyId);

        if (!shippingCompany.IsActive)
            throw new ShippingCompanyInactiveException(request.ShippingCompanyId);

        var shipment = Shipment.Create(request.OrderId, request.ShippingCompanyId);

        dbContext.Shipments.Add(shipment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return shipment.Id;
    }
}
