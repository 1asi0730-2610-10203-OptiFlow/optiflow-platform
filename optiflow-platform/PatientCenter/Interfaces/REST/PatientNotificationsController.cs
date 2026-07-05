using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.PatientCenter.Application.Errors;
using optiflow_platform.PatientCenter.Application.Services;
using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Domain.Model.Queries;
using optiflow_platform.PatientCenter.Interfaces.REST.Resources;
using optiflow_platform.PatientCenter.Interfaces.REST.Transform;
using optiflow_platform.Shared.Application.Patterns;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.PatientCenter.Interfaces.REST;

[ApiController]
[Route("api/v1/patients/{patientId}/notifications")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Patient Notifications")]
[Authorize]
[AllowWithoutSubscription]
public class PatientNotificationsController(
    IPatientNotificationQueryService notificationQueryService,
    IPatientNotificationCommandService notificationCommandService,
    ILogger<PatientNotificationsController> logger)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Gets notifications for a patient",
        OperationId = "GetNotificationsByPatientId")]
    [SwaggerResponse(200, "List of notifications",
        typeof(IEnumerable<PatientNotificationResource>))]
    public async Task<ActionResult> GetNotificationsByPatientId(int patientId,
        CancellationToken cancellationToken = default)
    {
        var query     = new GetNotificationsByPatientIdQuery(patientId);
        var result    = await notificationQueryService.Handle(query, cancellationToken);
        var resources = result.Select(PatientNotificationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Creates a notification for a patient",
        OperationId = "CreatePatientNotification")]
    [SwaggerResponse(201, "Notification created", typeof(PatientNotificationResource))]
    [SwaggerResponse(500, "Unexpected error")]
    public async Task<ActionResult> CreateNotification(int patientId,
        [FromBody] CreatePatientNotificationResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateNotificationCommandFromResourceAssembler
                .ToCommandFromResource(patientId, resource);
            var result = await notificationCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<PatientNotification, CreateNotificationError>.Success s =>
                    CreatedAtAction(nameof(GetNotificationsByPatientId),
                        new { patientId },
                        PatientNotificationResourceFromEntityAssembler.ToResourceFromEntity(s.Value)),
                _ => Problem(statusCode: 500)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating notification for patient {PatientId}", patientId);
            return Problem(statusCode: 500);
        }
    }

    [HttpPatch("{notificationId}/read")]
    [SwaggerOperation(Summary = "Marks a notification as read",
        OperationId = "MarkNotificationAsRead")]
    [SwaggerResponse(204, "Notification marked as read")]
    [SwaggerResponse(404, "Notification not found")]
    public async Task<ActionResult> MarkAsRead(int patientId, int notificationId,
        CancellationToken cancellationToken = default)
    {
        var query       = new GetNotificationsByPatientIdQuery(patientId);
        var all         = await notificationQueryService.Handle(query, cancellationToken);
        var notification = all.FirstOrDefault(n => n.Id == notificationId);
        if (notification is null) return NotFound();
        notification.MarkAsRead();
        return NoContent();
    }
}