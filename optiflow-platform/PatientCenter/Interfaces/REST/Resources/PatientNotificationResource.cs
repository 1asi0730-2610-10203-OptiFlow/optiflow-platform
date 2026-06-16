using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.PatientCenter.Interfaces.REST.Resources;

[SwaggerSchema(Description = "A patient notification resource")]
public record PatientNotificationResource(
    [SwaggerParameter("Notification ID")]   int      Id,
    [SwaggerParameter("Patient ID")]        int      PatientId,
    [SwaggerParameter("Notification text")] string   Message,
    [SwaggerParameter("Status")]            string   Status,
    [SwaggerParameter("Sent timestamp")]    DateTime SentAt);