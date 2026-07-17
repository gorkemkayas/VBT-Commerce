using BuildingBlocks.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Application.Services;
using Order.Domain.Exceptions;

namespace Order.Application.Commands.Me.CancelMyOrder;

public class CancelMyOrderCommandHandler(
    IOrderDbContext dbContext, ICurrentUserService currentUserService, OrderOperations orderOperations)
    : IRequestHandler<CancelMyOrderCommand, Unit>
{
    public async Task<Unit> Handle(CancelMyOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == userId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        await orderOperations.CancelAsync(order, "Cancelled by customer", cancellationToken);

        return Unit.Value;
    }
}
