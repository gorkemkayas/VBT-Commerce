using BuildingBlocks.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Application.Common;
using Order.Domain.Exceptions;

namespace Order.Application.Queries.Me.GetMyOrderById;

public class GetMyOrderByIdQueryHandler(IOrderDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<GetMyOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetMyOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.Coupons)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == userId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        return OrderMapper.ToDto(order);
    }
}
