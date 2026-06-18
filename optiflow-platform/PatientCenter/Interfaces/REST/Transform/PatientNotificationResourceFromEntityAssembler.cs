using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Interfaces.REST.Resources;

namespace optiflow_platform.PatientCenter.Interfaces.REST.Transform;

public static class PatientNotificationResourceFromEntityAssembler
{
    public static PatientNotificationResource ToResourceFromEntity(PatientNotification n) =>
        new(n.Id, n.PatientId, n.WorkOrderId, n.Message, n.Status, n.SentAt);
}