using optiflow_platform.Clinical.Application.Errors;
using optiflow_platform.Clinical.Application.Services;
using optiflow_platform.Clinical.Domain.Model.Commands;
using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Clinical.Domain.Repositories;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Clinical.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling prescription write operations.
/// </summary>
public class PrescriptionCommandService(
    IPrescriptionRepository prescriptionRepository,
    IClinicalRecordRepository clinicalRecordRepository,
    IUnitOfWork unitOfWork,
    ILogger<PrescriptionCommandService> logger)
    : IPrescriptionCommandService
{
    /// <inheritdoc />
    public async Task<Result<Prescription, CreatePrescriptionError>> Handle(
        CreatePrescriptionCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validate the parent record exists
        var record = await clinicalRecordRepository.FindByIdAsync(command.ClinicalRecordId, cancellationToken);
        if (record is null)
        {
            logger.LogWarning("Clinical record {RecordId} not found", command.ClinicalRecordId);
            return new Result<Prescription, CreatePrescriptionError>.Failure(
                CreatePrescriptionError.ClinicalRecordNotFound);
        }

        try
        {
            var prescription = new Prescription(command);
            await prescriptionRepository.AddAsync(prescription, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Prescription, CreatePrescriptionError>.Success(prescription);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while creating prescription for record {RecordId}", command.ClinicalRecordId);
            return new Result<Prescription, CreatePrescriptionError>.Failure(CreatePrescriptionError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating prescription for record {RecordId}", command.ClinicalRecordId);
            return new Result<Prescription, CreatePrescriptionError>.Failure(CreatePrescriptionError.UnexpectedError);
        }
    }
}
