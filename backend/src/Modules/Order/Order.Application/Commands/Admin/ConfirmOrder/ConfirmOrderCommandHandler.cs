using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Application.Services;
using Order.Domain.Exceptions;

namespace Order.Application.Commands.Admin.ConfirmOrder;

public class ConfirmOrderCommandHandler(IOrderDbContext dbContext, OrderOperations orderOperations) : IRequestHandler<ConfirmOrderCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.Include(o => o.Coupons).FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        await orderOperations.ConfirmAsync(order, cancellationToken);

        return Unit.Value;
    }
}
