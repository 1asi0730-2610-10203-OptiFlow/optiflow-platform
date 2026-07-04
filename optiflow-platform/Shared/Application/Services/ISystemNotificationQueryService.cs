using optiflow_platform.Shared.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Model.Queries;

namespace optiflow_platform.Shared.Application.Services;

public interface ISystemNotificationQueryService
{
    Task<IEnumerable<SystemNotification>> Handle(GetNotificationsByRecipientUserIdQuery query,
        CancellationToken cancellationToken = default);
}
