using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Model.Commands;
using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Clinical.Interfaces.REST.Resources;

namespace optiflow_platform.Clinical.Interfaces.REST.Transform;

public static class CreatePatientCommandFromResourceAssembler
{
    public static CreatePatientCommand ToCommandFromResource(CreatePatientResource resource) =>
        new(resource.FirstName,
            resource.LastName,
            resource.Dni,
            resource.Phone,
            resource.Email,
            DateOnly.Parse(resource.BirthDate));
}

public static class UpdatePatientCommandFromResourceAssembler
{
    public static UpdatePatientCommand ToCommandFromResource(int patientId, UpdatePatientResource resource) =>
        new(patientId,
            resource.FirstName,
            resource.LastName ?? "",
            resource.Dni,
            resource.Phone,
            resource.Email,
            DateOnly.Parse(resource.BirthDate));
}

public static class PatientResourceFromEntityAssembler
{
    public static PatientResource ToResourceFromEntity(Patient entity) =>
        new(entity.Id,
            entity.PatientId,        // alias → mismo valor que Id
            entity.CustomerUuid,
            entity.FirstName,
            entity.LastName,
            entity.Dni,
            entity.Phone,
            entity.Email,
            entity.BirthDate.ToString("yyyy-MM-dd"));
}

public static class ClinicalRecordResourceFromEntityAssembler
{
    public static ClinicalRecordResource ToResourceFromEntity(ClinicalRecord entity) =>
        new(entity.Id,
            entity.RecordId,             // alias → mismo valor que Id
            entity.ClinicalRecordUuid,
            entity.PatientId);
}

public static class CreatePrescriptionCommandFromResourceAssembler
{
    public static CreatePrescriptionCommand ToCommandFromResource(CreatePrescriptionResource resource) =>
        new(resource.ClinicalRecordId,
            resource.OdSphere,
            resource.OdCylinder,
            resource.OdAxis,
            resource.OiSphere,
            resource.OiCylinder,
            resource.OiAxis,
            resource.Addition,
            resource.Notes,
            resource.DoctorName);
}

public static class PrescriptionResourceFromEntityAssembler
{
    public static PrescriptionResource ToResourceFromEntity(Prescription entity) =>
        new(entity.Id,
            entity.PrescriptionId,       // alias → mismo valor que Id
            entity.PrescriptionUuid,
            entity.ClinicalRecordId,
            entity.OdSphere,
            entity.OdCylinder,
            entity.OdAxis,
            entity.OiSphere,
            entity.OiCylinder,
            entity.OiAxis,
            entity.Addition,
            entity.Notes,
            entity.DoctorName,
            entity.CreatedAt.ToString("o"));
}