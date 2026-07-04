namespace optiflow_platform.Shared.Domain.Model.Commands;

public record CreateSystemNotificationCommand(
    int    RecipientUserId,
    string Category,
    string Message,
    Guid   AccountId);
