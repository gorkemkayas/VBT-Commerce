using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Application.Common;
using Order.Domain.Exceptions;

namespace Order.Application.Queries.Admin.GetOrderById;

public class GetOrderByIdQueryHandler(IOrderDbContext dbContext) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.Coupons)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        return OrderMapper.ToDto(order);
    }
}
