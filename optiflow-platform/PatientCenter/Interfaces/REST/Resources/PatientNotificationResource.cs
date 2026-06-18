using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.PatientCenter.Interfaces.REST.Resources;

[SwaggerSchema(Description = "A patient notification resource")]
public record PatientNotificationResource(
    [SwaggerParameter("Notification ID")]   int       Id,
    [SwaggerParameter("Patient ID")]        int       PatientId,
    [SwaggerParameter("Work order ID")]     int       WorkOrderId,
    [SwaggerParameter("Notification text")] string    Message,
    [SwaggerParameter("Status")]            string    Status,
    [SwaggerParameter("Sent timestamp")]    DateTime? SentAt);

[SwaggerSchema(Description = "Payload to create a notification")]
public record CreatePatientNotificationResource(
    [Required][SwaggerParameter("Work order ID")]       int    WorkOrderId,
    [Required][SwaggerParameter("Notification message")] string Message);