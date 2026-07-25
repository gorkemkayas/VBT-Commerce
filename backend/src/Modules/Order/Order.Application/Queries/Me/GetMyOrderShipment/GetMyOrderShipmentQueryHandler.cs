using BuildingBlocks.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Application.Integrations;
using Order.Domain.Exceptions;
using Shipping.Contracts;

namespace Order.Application.Queries.Me.GetMyOrderShipment;

public class GetMyOrderShipmentQueryHandler(
    IOrderDbContext dbContext,
    ICurrentUserService currentUserService,
    IShippingIntegrationService shippingIntegrationService)
    : IRequestHandler<GetMyOrderShipmentQuery, ShipmentTrackingDto>
{
    public async Task<ShipmentTrackingDto> Handle(GetMyOrderShipmentQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var orderExists = await dbContext.Orders
            .AsNoTracking()
            .AnyAsync(o => o.Id == request.OrderId && o.UserId == userId, cancellationToken);

        if (!orderExists)
            throw new OrderNotFoundException(request.OrderId);

        return await shippingIntegrationService.GetShipmentByOrderIdAsync(request.OrderId, cancellationToken)
            ?? throw new OrderShipmentNotFoundException(request.OrderId);
    }
}
