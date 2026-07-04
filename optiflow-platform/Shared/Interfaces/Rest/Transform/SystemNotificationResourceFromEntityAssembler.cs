using optiflow_platform.Shared.Domain.Model.Entities;
using optiflow_platform.Shared.Interfaces.Rest.Resources;

namespace optiflow_platform.Shared.Interfaces.Rest.Transform;

public static class SystemNotificationResourceFromEntityAssembler
{
    public static SystemNotificationResource ToResourceFromEntity(SystemNotification n) =>
        new(n.Id, n.RecipientUserId, n.Category, n.Message, n.IsRead, n.CreatedAt);
}
