using BuildingBlocks.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Application.Services;
using Order.Domain.Exceptions;

namespace Order.Application.Commands.Admin.CancelOrder;

public class CancelOrderCommandHandler(IOrderDbContext dbContext, ICurrentUserService currentUserService, OrderOperations orderOperations)
    : IRequestHandler<CancelOrderCommand, Unit>
{
    public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        await orderOperations.CancelAsync(order, request.Reason, currentUserService.IpAddress ?? "0.0.0.0", cancellationToken);

        return Unit.Value;
    }
}
