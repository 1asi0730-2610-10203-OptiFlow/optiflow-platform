using optiflow_platform.Clinical.Application.Errors;
using optiflow_platform.Clinical.Application.Services;
using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Model.Commands;
using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Clinical.Domain.Repositories;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Clinical.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling patient write operations.
/// </summary>
/// <remarks>
///     When a patient is created, a linked <see cref="ClinicalRecord"/> is automatically
///     created in the same transaction to ensure every patient always has a record.
/// </remarks>
public class PatientCommandService(
    IPatientRepository patientRepository,
    IClinicalRecordRepository clinicalRecordRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserContext currentUserContext,
    optiflow_platform.IAM.Application.ACL.IClientAccountService clientAccountService,
    ILogger<PatientCommandService> logger)
    : IPatientCommandService
{
    /// <inheritdoc />
    public async Task<Result<Patient, CreatePatientError>> Handle(
        CreatePatientCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Enforce unique DNI
            var existing = await patientRepository.FindByDniAsync(command.Dni, cancellationToken);
            if (existing is not null)
            {
                logger.LogWarning("Patient with DNI {Dni} already exists", command.Dni);
                return new Result<Patient, CreatePatientError>.Failure(CreatePatientError.DniAlreadyExists);
            }

            var patient = new Patient(command, currentUserContext.AccountId!.Value);
            await patientRepository.AddAsync(patient, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken); // flush to get patient.Id

            // Auto-create the linked clinical record
            var record = new ClinicalRecord(patient.Id, patient.AccountId);
            await clinicalRecordRepository.AddAsync(record, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            // Turn the patient into a valid client user of this optic so they can log in (passwordless)
            // and see their orders. Best-effort — never fail patient creation over it.
            if (!string.IsNullOrWhiteSpace(command.Email))
            {
                try
                {
                    await clientAccountService.EnsureClientUserAsync(command.Email!, patient.AccountId, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Could not provision client user for patient {Dni}", command.Dni);
                }
            }

            return new Result<Patient, CreatePatientError>.Success(patient);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while creating patient {Dni}", command.Dni);
            return new Result<Patient, CreatePatientError>.Failure(CreatePatientError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating patient {Dni}", command.Dni);
            return new Result<Patient, CreatePatientError>.Failure(CreatePatientError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Patient, UpdatePatientError>> Handle(
        UpdatePatientCommand command,
        CancellationToken cancellationToken = default)
    {
        var patient = await patientRepository.FindByIdAsync(command.PatientId, cancellationToken);
        if (patient is null)
        {
            logger.LogWarning("Patient {PatientId} not found for update", command.PatientId);
            return new Result<Patient, UpdatePatientError>.Failure(UpdatePatientError.PatientNotFound);
        }

        try
        {
            // Check DNI conflict only if it changed
            if (!patient.Dni.Equals(command.Dni, StringComparison.OrdinalIgnoreCase))
            {
                var conflict = await patientRepository.FindByDniAsync(command.Dni, cancellationToken);
                if (conflict is not null)
                    return new Result<Patient, UpdatePatientError>.Failure(UpdatePatientError.DniAlreadyExists);
            }

            patient.Update(command);
            patientRepository.Update(patient);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Patient, UpdatePatientError>.Success(patient);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while updating patient {PatientId}", command.PatientId);
            return new Result<Patient, UpdatePatientError>.Failure(UpdatePatientError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while updating patient {PatientId}", command.PatientId);
            return new Result<Patient, UpdatePatientError>.Failure(UpdatePatientError.UnexpectedError);
        }
    }
}
