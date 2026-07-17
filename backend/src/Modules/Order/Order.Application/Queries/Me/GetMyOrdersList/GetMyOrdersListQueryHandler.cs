using BuildingBlocks.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Application.Common;

namespace Order.Application.Queries.Me.GetMyOrdersList;

public class GetMyOrdersListQueryHandler(IOrderDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<GetMyOrdersListQuery, PagedResult<OrderDto>>
{
    public async Task<PagedResult<OrderDto>> Handle(GetMyOrdersListQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var query = dbContext.Orders.AsNoTracking().Where(o => o.UserId == userId);

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
