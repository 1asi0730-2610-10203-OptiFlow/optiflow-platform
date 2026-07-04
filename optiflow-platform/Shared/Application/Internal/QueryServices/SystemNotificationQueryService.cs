using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Model.Queries;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Shared.Application.Internal.QueryServices;

public class SystemNotificationQueryService(ISystemNotificationRepository notificationRepository)
    : ISystemNotificationQueryService
{
    public async Task<IEnumerable<SystemNotification>> Handle(GetNotificationsByRecipientUserIdQuery query,
        CancellationToken cancellationToken = default) =>
        await notificationRepository.FindByRecipientUserIdAsync(query.RecipientUserId, cancellationToken);
}
