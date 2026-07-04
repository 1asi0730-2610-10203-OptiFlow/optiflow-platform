using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Application.Errors;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Model.Commands;
using optiflow_platform.Shared.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Shared.Application.Internal.CommandServices;

public class SystemNotificationCommandService(
    ISystemNotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    ILogger<SystemNotificationCommandService> logger)
    : ISystemNotificationCommandService
{
    public async Task<Result<SystemNotification, CreateSystemNotificationError>> Handle(
        CreateSystemNotificationCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new SystemNotification(command);
            await notificationRepository.AddAsync(notification, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<SystemNotification, CreateSystemNotificationError>.Success(notification);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while creating system notification for user {RecipientUserId}",
                command.RecipientUserId);
            return new Result<SystemNotification, CreateSystemNotificationError>.Failure(
                CreateSystemNotificationError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating system notification for user {RecipientUserId}",
                command.RecipientUserId);
            return new Result<SystemNotification, CreateSystemNotificationError>.Failure(
                CreateSystemNotificationError.UnexpectedError);
        }
    }
}
