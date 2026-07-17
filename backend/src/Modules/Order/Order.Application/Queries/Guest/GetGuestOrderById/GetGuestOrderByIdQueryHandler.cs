using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Application.Common;
using Order.Domain.Exceptions;

namespace Order.Application.Queries.Guest.GetGuestOrderById;

public class GetGuestOrderByIdQueryHandler(IOrderDbContext dbContext) : IRequestHandler<GetGuestOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetGuestOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.Coupons)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.GuestCustomerId == request.GuestCustomerId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        return OrderMapper.ToDto(order);
    }
}
