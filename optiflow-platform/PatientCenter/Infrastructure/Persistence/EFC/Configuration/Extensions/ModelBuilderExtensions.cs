using Microsoft.EntityFrameworkCore;
using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace optiflow_platform.PatientCenter.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyPatientCenterConfiguration(this ModelBuilder builder, AppDbContext dbContext)
    {
        // LensMaterial is a shared reference catalog (no write path) — not account-owned data.
        builder.Entity<LensMaterial>().HasKey(l => l.Id);
        builder.Entity<LensMaterial>().Property(l => l.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<LensMaterial>().Property(l => l.FullName).IsRequired().HasMaxLength(255);
        builder.Entity<LensMaterial>().Property(l => l.IndexValue).IsRequired().HasMaxLength(50);
        builder.Entity<LensMaterial>().Property(l => l.BasePrice).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<LensMaterial>().Property(l => l.Description).HasMaxLength(500);

        builder.Entity<PatientNotification>().HasKey(n => n.Id);
        builder.Entity<PatientNotification>().Property(n => n.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<PatientNotification>().Property(n => n.AccountId).IsRequired();
        builder.Entity<PatientNotification>().HasIndex(n => n.AccountId);
        builder.Entity<PatientNotification>().HasQueryFilter(n => n.AccountId == dbContext.CurrentAccountId);
        builder.Entity<PatientNotification>().Property(n => n.PatientId).IsRequired();
        builder.Entity<PatientNotification>().Property(n => n.Message).IsRequired().HasMaxLength(500);
        builder.Entity<PatientNotification>().Property(n => n.Status).IsRequired().HasMaxLength(20);
        builder.Entity<PatientNotification>().Property(n => n.SentAt).IsRequired();
    }
}
