using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.PatientCenter.Application.Services;
using optiflow_platform.PatientCenter.Domain.Model.Queries;
using optiflow_platform.PatientCenter.Interfaces.REST.Resources;
using optiflow_platform.PatientCenter.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.PatientCenter.Interfaces.REST;

[ApiController]
[Route("api/v1/patients/{patientId}/notifications")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Patient Notifications")]
public class PatientNotificationsController(IPatientNotificationQueryService notificationQueryService)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Gets notifications for a patient", OperationId = "GetNotificationsByPatientId")]
    [SwaggerResponse(200, "List of notifications", typeof(IEnumerable<PatientNotificationResource>))]
    public async Task<ActionResult> GetNotificationsByPatientId(int patientId,
        CancellationToken cancellationToken = default)
    {
        var query     = new GetNotificationsByPatientIdQuery(patientId);
        var result    = await notificationQueryService.Handle(query, cancellationToken);
        var resources = result.Select(PatientNotificationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}