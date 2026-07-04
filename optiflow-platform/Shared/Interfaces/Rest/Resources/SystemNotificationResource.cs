using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Shared.Interfaces.Rest.Resources;

[SwaggerSchema(Description = "A system notification resource")]
public record SystemNotificationResource(
    [SwaggerParameter("Notification ID")]   int      Id,
    [SwaggerParameter("Recipient user ID")] int      RecipientUserId,
    [SwaggerParameter("Category")]          string   Category,
    [SwaggerParameter("Notification text")] string   Message,
    [SwaggerParameter("Read status")]       bool     IsRead,
    [SwaggerParameter("Created timestamp")] DateTime CreatedAt);
