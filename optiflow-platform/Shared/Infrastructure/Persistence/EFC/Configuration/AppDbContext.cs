using Microsoft.EntityFrameworkCore;
using optiflow_platform.Analytics.Infrastructure.Persistence.EFC.Configuration.Extensions;
using optiflow_platform.Clinical.Infrastructure.Persistence.EFC.Configuration.Extensions;
using optiflow_platform.Inventory.Infrastructure.Persistence.EFC.Configuration.Extensions;
using optiflow_platform.LabAndOrders.Infrastructure.Persistence.EFC.Configuration.Extensions;
using optiflow_platform.PatientCenter.Infrastructure.Persistence.EFC.Configuration.Extensions;
using optiflow_platform.Sales.Infrastructure.Persistence.EFC.Configuration.Extensions;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Interceptors;
using optiflow_platform.Subscription.Infrastructure.Persistence.EFC.Configuration.Extensions;
using optiflow_platform.IAM.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
///     Application database context
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplySalesConfiguration();
        builder.ApplySubscriptionConfiguration();
        builder.ApplyInventoryConfiguration();
        builder.ApplyClinicalConfiguration();
        builder.ApplyAnalyticsConfiguration();
        builder.ApplyLabAndOrdersConfiguration();
        builder.ApplyPatientCenterConfiguration();
        builder.ApplyIamConfiguration();

        builder.UseSnakeCaseNamingConvention();
    }
}
