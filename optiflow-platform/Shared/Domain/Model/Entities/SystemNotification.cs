using optiflow_platform.Shared.Domain.Model.Commands;

namespace optiflow_platform.Shared.Domain.Model.Entities;

/// <summary>
///     A generic, cross-context notification addressed to a specific IAM user (e.g. staff/admin),
///     as opposed to <c>PatientCenter.PatientNotification</c> which is scoped to a patient and work order.
/// </summary>
public class SystemNotification
{
    protected SystemNotification()
    {
        Category = null!;
        Message  = null!;
    }

    public SystemNotification(CreateSystemNotificationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        RecipientUserId = command.RecipientUserId;
        Category        = command.Category;
        Message         = command.Message;
        IsRead          = false;
        CreatedAt       = DateTime.UtcNow;
    }

    public int      Id              { get; private set; }
    public int      RecipientUserId { get; private set; }
    public string   Category        { get; private set; }
    public string   Message         { get; private set; }
    public bool     IsRead          { get; private set; }
    public DateTime CreatedAt       { get; private set; }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
