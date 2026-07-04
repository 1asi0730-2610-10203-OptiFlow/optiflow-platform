using optiflow_platform.Shared.Application.Errors;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Domain.Model.Commands;
using optiflow_platform.Shared.Domain.Model.Entities;

namespace optiflow_platform.Shared.Application.Services;

public interface ISystemNotificationCommandService
{
    Task<Result<SystemNotification, CreateSystemNotificationError>> Handle(
        CreateSystemNotificationCommand command,
        CancellationToken cancellationToken = default);
}
