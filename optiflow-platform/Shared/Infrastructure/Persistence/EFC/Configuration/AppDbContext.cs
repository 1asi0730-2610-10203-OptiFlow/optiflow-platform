using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Entities;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Interceptors;
using Microsoft.EntityFrameworkCore;

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

        // Lab and Orders Bounded Context
        builder.Entity<Laboratory>().HasKey(l => l.Id);
        builder.Entity<Laboratory>().Property(l => l.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Laboratory>().Property(l => l.Name).IsRequired().HasMaxLength(255);
        builder.Entity<Laboratory>().Property(l => l.ContactInfo).HasMaxLength(500);

        builder.Entity<WorkOrder>().HasKey(w => w.Id);
        builder.Entity<WorkOrder>().Property(w => w.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<WorkOrder>().Property(w => w.SaleId).IsRequired();
        builder.Entity<WorkOrder>().Property(w => w.RecipeId).IsRequired();
        builder.Entity<WorkOrder>().Property(w => w.LabId).IsRequired();
        builder.Entity<WorkOrder>().Property(w => w.Status).IsRequired().HasMaxLength(50);
        builder.Entity<WorkOrder>().Property(w => w.Priority).IsRequired().HasMaxLength(20);
        builder.Entity<WorkOrder>().Property(w => w.PatientName).IsRequired().HasMaxLength(255);
        builder.Entity<WorkOrder>().Property(w => w.LaboratoryName).IsRequired().HasMaxLength(255);
        builder.Entity<WorkOrder>().Property(w => w.LensType).HasMaxLength(100);
        builder.Entity<WorkOrder>().Property(w => w.Frame).HasMaxLength(255);
        builder.Entity<WorkOrder>().Property(w => w.Prescription).HasMaxLength(500);
        builder.Entity<WorkOrder>().Property(w => w.DeliveryDate).HasMaxLength(50);
        builder.Entity<WorkOrder>().Property(w => w.Deposit).HasColumnType("decimal(10,2)");
        builder.Entity<WorkOrder>().Property(w => w.Total).HasColumnType("decimal(10,2)");
        builder.Entity<WorkOrder>().Property(w => w.IsRework).IsRequired();

        builder.UseSnakeCaseNamingConvention();
    }
}
