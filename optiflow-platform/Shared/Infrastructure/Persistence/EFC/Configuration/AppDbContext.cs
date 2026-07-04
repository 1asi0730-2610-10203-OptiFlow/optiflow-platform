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
    /// <summary>
    ///     The account the current request belongs to, set once per request by <c>JwtMiddleware</c>
    ///     right after resolving the caller. Bounded-context <c>Apply*Configuration</c> extensions
    ///     use this via global query filters to keep every tenant-owned entity scoped to it. Kept as a
    ///     plain mutable property (not a constructor dependency) so `dotnet ef migrations add` can still
    ///     construct this context at design time without needing an HTTP request in scope.
    /// </summary>
    public Guid? CurrentAccountId { get; set; }

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

        builder.ApplySharedConfiguration(this);
        builder.ApplySalesConfiguration(this);
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
