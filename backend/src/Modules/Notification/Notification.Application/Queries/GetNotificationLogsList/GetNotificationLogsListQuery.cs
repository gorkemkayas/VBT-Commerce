using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Notification.Application.Common;

namespace Notification.Application.Queries.GetNotificationLogsList;

public record GetNotificationLogsListQuery(bool? IsSuccess = null, int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResult<NotificationLogDto>>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
