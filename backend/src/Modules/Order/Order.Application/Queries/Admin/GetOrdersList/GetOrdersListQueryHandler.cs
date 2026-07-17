using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Application.Common;

namespace Order.Application.Queries.Admin.GetOrdersList;

public class GetOrdersListQueryHandler(IOrderDbContext dbContext) : IRequestHandler<GetOrdersListQuery, PagedResult<OrderDto>>
{
    public async Task<PagedResult<OrderDto>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Orders.AsNoTracking();

        if (request.Status is not null)
            query = query.Where(o => o.Status == request.Status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(o => o.Items)
            .Include(o => o.Coupons)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<OrderDto>(items.Select(OrderMapper.ToDto).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
