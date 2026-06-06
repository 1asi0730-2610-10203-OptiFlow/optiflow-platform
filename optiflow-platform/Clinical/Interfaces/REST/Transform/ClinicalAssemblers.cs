using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Model.Commands;
using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Clinical.Interfaces.REST.Resources;

namespace optiflow_platform.Clinical.Interfaces.REST.Transform;

/// <summary>Assembler between <see cref="CreatePatientResource"/> and <see cref="CreatePatientCommand"/>.</summary>
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

/// <summary>Assembler between <see cref="UpdatePatientResource"/> and <see cref="UpdatePatientCommand"/>.</summary>
public static class UpdatePatientCommandFromResourceAssembler
{
    public static UpdatePatientCommand ToCommandFromResource(int patientId, UpdatePatientResource resource) =>
        new(patientId,
            resource.FirstName,
            resource.LastName,
            resource.Dni,
            resource.Phone,
            resource.Email,
            DateOnly.Parse(resource.BirthDate));
}

/// <summary>Assembler that converts a <see cref="Patient"/> entity to a <see cref="PatientResource"/>.</summary>
public static class PatientResourceFromEntityAssembler
{
    public static PatientResource ToResourceFromEntity(Patient entity) =>
        new(entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.Dni,
            entity.Phone,
            entity.Email,
            entity.BirthDate.ToString("yyyy-MM-dd"));
}

/// <summary>Assembler that converts a <see cref="ClinicalRecord"/> entity to a <see cref="ClinicalRecordResource"/>.</summary>
public static class ClinicalRecordResourceFromEntityAssembler
{
    public static ClinicalRecordResource ToResourceFromEntity(ClinicalRecord entity) =>
        new(entity.Id, entity.PatientId);
}

/// <summary>Assembler between <see cref="CreatePrescriptionResource"/> and <see cref="CreatePrescriptionCommand"/>.</summary>
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

/// <summary>Assembler that converts a <see cref="Prescription"/> entity to a <see cref="PrescriptionResource"/>.</summary>
public static class PrescriptionResourceFromEntityAssembler
{
    public static PrescriptionResource ToResourceFromEntity(Prescription entity) =>
        new(entity.Id,
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
