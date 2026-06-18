using Microsoft.EntityFrameworkCore;
using optiflow_platform.Analytics.Domain.Model.Aggregates;
using optiflow_platform.Analytics.Domain.Model.Entities;

namespace optiflow_platform.Analytics.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyAnalyticsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<AnalyticsReport>().HasKey(a => a.Id);
        builder.Entity<AnalyticsReport>().Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AnalyticsReport>().Property(a => a.GeneratedBy).IsRequired().HasMaxLength(255);
        builder.Entity<AnalyticsReport>().Property(a => a.Period).IsRequired().HasMaxLength(7);
        builder.Entity<AnalyticsReport>().Property(a => a.GeneratedAt).IsRequired();
        builder.Entity<AnalyticsReport>().Property(a => a.TotalRevenue).HasColumnType("decimal(15,2)");
        builder.Entity<AnalyticsReport>().Property(a => a.ConversionRate).HasColumnType("decimal(5,4)");
        builder.Entity<AnalyticsReport>().Property(a => a.AverageDeliveryDays).HasColumnType("decimal(6,2)");
        builder.Entity<AnalyticsReport>().Property(a => a.OnTimeDeliveryRate).HasColumnType("decimal(5,4)");
        builder.Entity<AnalyticsReport>().Property(a => a.ReworkRate).HasColumnType("decimal(5,4)");
        builder.Entity<AnalyticsReport>().Property(a => a.PendingBalance0To7).HasColumnType("decimal(15,2)");
        builder.Entity<AnalyticsReport>().Property(a => a.PendingBalance8To15).HasColumnType("decimal(15,2)");
        builder.Entity<AnalyticsReport>().Property(a => a.PendingBalance16To30).HasColumnType("decimal(15,2)");
        builder.Entity<AnalyticsReport>().Property(a => a.PendingBalanceOver30).HasColumnType("decimal(15,2)");

        builder.Entity<StaffMetric>().HasKey(s => s.Id);
        builder.Entity<StaffMetric>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<StaffMetric>().Property(s => s.ReportId).IsRequired();
        builder.Entity<StaffMetric>().Property(s => s.EmployeeName).IsRequired().HasMaxLength(255);
        builder.Entity<StaffMetric>().Property(s => s.TotalRevenue).HasColumnType("decimal(15,2)");
        builder.Entity<StaffMetric>()
            .HasOne<AnalyticsReport>()
            .WithMany()
            .HasForeignKey(s => s.ReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
