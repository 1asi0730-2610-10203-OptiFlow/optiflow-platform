namespace optiflow_platform.PatientCenter.Domain.Model.Commands;

public record CreateNotificationCommand(int PatientId, string Message);