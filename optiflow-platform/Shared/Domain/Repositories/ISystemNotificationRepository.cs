using optiflow_platform.Shared.Domain.Model.Entities;

namespace optiflow_platform.Shared.Domain.Repositories;

public interface ISystemNotificationRepository : IBaseRepository<SystemNotification>
{
    Task<IEnumerable<SystemNotification>> FindByRecipientUserIdAsync(
        int recipientUserId, CancellationToken cancellationToken = default);
}
