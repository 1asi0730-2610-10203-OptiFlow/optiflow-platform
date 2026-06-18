using optiflow_platform.PatientCenter.Domain.Model.Commands;
using optiflow_platform.PatientCenter.Interfaces.REST.Resources;

namespace optiflow_platform.PatientCenter.Interfaces.REST.Transform;

public static class CreateNotificationCommandFromResourceAssembler
{
    public static CreateNotificationCommand ToCommandFromResource(
        int patientId, CreatePatientNotificationResource resource) =>
        new(patientId, resource.WorkOrderId, resource.Message);
}