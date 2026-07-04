using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Model.Queries;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Shared.Interfaces.Rest.Resources;
using optiflow_platform.Shared.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.Shared.Interfaces.Rest;

[ApiController]
[Route("api/v1/users/{userId}/notifications")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("System Notifications")]
[Authorize]
public class SystemNotificationsController(
    ISystemNotificationQueryService notificationQueryService,
    ISystemNotificationRepository notificationRepository,
    IUnitOfWork unitOfWork)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Gets notifications for a user", OperationId = "GetNotificationsByUserId")]
    [SwaggerResponse(200, "List of notifications", typeof(IEnumerable<SystemNotificationResource>))]
    public async Task<ActionResult> GetNotificationsByUserId(int userId, CancellationToken cancellationToken = default)
    {
        var query     = new GetNotificationsByRecipientUserIdQuery(userId);
        var result    = await notificationQueryService.Handle(query, cancellationToken);
        var resources = result.Select(SystemNotificationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPatch("{notificationId}/read")]
    [SwaggerOperation(Summary = "Marks a notification as read", OperationId = "MarkSystemNotificationAsRead")]
    [SwaggerResponse(204, "Notification marked as read")]
    [SwaggerResponse(404, "Notification not found")]
    public async Task<ActionResult> MarkAsRead(int userId, int notificationId,
        CancellationToken cancellationToken = default)
    {
        var query        = new GetNotificationsByRecipientUserIdQuery(userId);
        var all          = await notificationQueryService.Handle(query, cancellationToken);
        var notification = all.FirstOrDefault(n => n.Id == notificationId);
        if (notification is null) return NotFound();

        notification.MarkAsRead();
        notificationRepository.Update(notification);
        await unitOfWork.CompleteAsync(cancellationToken);
        return NoContent();
    }
}
