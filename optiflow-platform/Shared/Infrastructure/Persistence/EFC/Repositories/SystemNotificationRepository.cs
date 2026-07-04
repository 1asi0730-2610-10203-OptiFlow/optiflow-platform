using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;

public class SystemNotificationRepository(AppDbContext context)
    : BaseRepository<SystemNotification>(context), ISystemNotificationRepository
{
    public async Task<IEnumerable<SystemNotification>> FindByRecipientUserIdAsync(
        int recipientUserId, CancellationToken cancellationToken = default) =>
        await Context.Set<SystemNotification>()
            .Where(n => n.RecipientUserId == recipientUserId)
            .ToListAsync(cancellationToken);
}
