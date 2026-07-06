using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Staff.Domain.Model.Aggregates;

namespace optiflow_platform.Staff.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyStaffConfiguration(this ModelBuilder builder, AppDbContext dbContext)
    {
        builder.Entity<StaffMember>(entity =>
        {
            entity.ToTable("staff");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.AccountId).IsRequired();
            entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(100);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntryDate).HasMaxLength(50);
            entity.HasIndex(e => new { e.AccountId, e.EmployeeCode }).IsUnique();
            entity.HasQueryFilter(e => e.AccountId == dbContext.CurrentAccountId);
        });

        builder.Entity<Role>(entity =>
        {
            entity.ToTable("staff_roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.AccountId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Permissions).IsRequired().HasMaxLength(1000);
            entity.HasIndex(e => new { e.AccountId, e.Name }).IsUnique();
            entity.HasQueryFilter(e => e.AccountId == dbContext.CurrentAccountId);
        });
    }
}
