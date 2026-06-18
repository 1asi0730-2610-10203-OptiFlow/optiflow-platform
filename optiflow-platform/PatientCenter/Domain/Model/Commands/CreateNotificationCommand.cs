namespace optiflow_platform.PatientCenter.Domain.Model.Commands;

public record CreateNotificationCommand(
    int    PatientId,
    int    WorkOrderId,
    string Message);