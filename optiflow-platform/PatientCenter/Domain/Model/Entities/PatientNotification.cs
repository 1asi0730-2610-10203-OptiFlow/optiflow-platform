using optiflow_platform.PatientCenter.Domain.Model.Commands;
using optiflow_platform.PatientCenter.Domain.Model.ValueObjects;

namespace optiflow_platform.PatientCenter.Domain.Model.Entities;

public class PatientNotification
{
    protected PatientNotification()
    {
        Message = null!;
        Status  = null!;
    }

    public PatientNotification(CreateNotificationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        PatientId = command.PatientId;
        Message   = command.Message;
        Status    = NotificationStatus.Pending;
        SentAt    = DateTime.UtcNow;
    }

    public int      Id        { get; private set; }
    public int      PatientId { get; private set; }
    public string   Message   { get; private set; }
    public string   Status    { get; private set; }
    public DateTime SentAt    { get; private set; }

    public void MarkAsRead()
    {
        Status = NotificationStatus.Read;
    }
}