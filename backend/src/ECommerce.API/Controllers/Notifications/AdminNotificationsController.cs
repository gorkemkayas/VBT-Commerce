using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common;
using Notification.Application.Queries.GetNotificationLogsList;

namespace ECommerce.API.Controllers.Notifications;

[ApiController]
[Route("api/admin/notifications")]
public class AdminNotificationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<NotificationLogDto>>> GetList(
        [FromQuery] bool? isSuccess, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetNotificationLogsListQuery(isSuccess, pageNumber == 0 ? 1 : pageNumber, pageSize == 0 ? 20 : pageSize),
            cancellationToken);

        return Ok(result);
    }
}
