using Microsoft.EntityFrameworkCore;
using optiflow_platform.PatientCenter.Application.Errors;
using optiflow_platform.PatientCenter.Application.Services;
using optiflow_platform.PatientCenter.Domain.Model.Commands;
using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Domain.Repositories;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.PatientCenter.Application.Internal.CommandServices;

public class PatientNotificationCommandService(
    IPatientNotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserContext currentUserContext,
    ILogger<PatientNotificationCommandService> logger)
    : IPatientNotificationCommandService
{
    public async Task<Result<PatientNotification, CreateNotificationError>> Handle(
        CreateNotificationCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new PatientNotification(command, currentUserContext.AccountId!.Value);
            await notificationRepository.AddAsync(notification, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<PatientNotification, CreateNotificationError>.Success(notification);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while creating notification for patient {PatientId}",
                command.PatientId);
            return new Result<PatientNotification, CreateNotificationError>.Failure(
                CreateNotificationError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating notification for patient {PatientId}",
                command.PatientId);
            return new Result<PatientNotification, CreateNotificationError>.Failure(
                CreateNotificationError.UnexpectedError);
        }
    }
}