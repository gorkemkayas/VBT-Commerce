using MediatR;
using Microsoft.EntityFrameworkCore;
using Notification.Application.Abstractions;
using Notification.Application.Common;

namespace Notification.Application.Queries.GetNotificationLogsList;

public class GetNotificationLogsListQueryHandler(INotificationDbContext dbContext)
    : IRequestHandler<GetNotificationLogsListQuery, PagedResult<NotificationLogDto>>
{
    public async Task<PagedResult<NotificationLogDto>> Handle(GetNotificationLogsListQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.NotificationLogs.AsNoTracking();

        if (request.IsSuccess is not null)
            query = query.Where(l => l.IsSuccess == request.IsSuccess);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<NotificationLogDto>(items.Select(NotificationLogMapper.ToDto).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
