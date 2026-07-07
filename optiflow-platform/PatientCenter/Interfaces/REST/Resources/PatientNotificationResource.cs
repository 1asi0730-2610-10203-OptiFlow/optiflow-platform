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
    [Required][Range(1, int.MaxValue, ErrorMessage = "WorkOrderId must be a positive identifier")][SwaggerParameter("Work order ID")] int WorkOrderId,
    [Required][StringLength(500, ErrorMessage = "Message must be at most 500 characters")][SwaggerParameter("Notification message")] string Message);