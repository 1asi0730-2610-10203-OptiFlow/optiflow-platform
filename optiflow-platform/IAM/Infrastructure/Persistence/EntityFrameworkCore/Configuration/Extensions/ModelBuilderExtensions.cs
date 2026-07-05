using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Model.Entities;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.IAM.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyIamConfiguration(this ModelBuilder builder)
    {
        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasConversion(v => v.Value, v => new UserId(v)).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.Email).HasConversion(v => v.Value, v => new EmailAddress(v)).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Password).HasConversion(v => v.Value, v => new Password(v)).IsRequired().HasColumnName("PasswordHash");
            entity.Property(e => e.GoogleId).HasConversion(v => v != null ? v.Value : null, v => v != null ? new GoogleId(v) : null).HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired()
                .HasConversion(
                    v => v.ToString().ToUpper(),
                    v => (UserStatus)System.Enum.Parse(typeof(UserStatus), v, true)
                );

            entity.Property(e => e.Role).IsRequired().HasMaxLength(20)
                .HasConversion(
                    v => v.ToString().ToUpper(),
                    v => (UserRole)System.Enum.Parse(typeof(UserRole), v, true)
                );

            entity.Property(e => e.AccountId);

            entity.Property(e => e.Version).IsConcurrencyToken();
            entity.HasQueryFilter(e => e.DeletedAt == null);

            entity.HasIndex(e => e.Email).IsUnique();
        });

        builder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.OwnerUserId);
        });

        builder.Entity<PasswordRecoveryToken>(entity =>
        {
            entity.ToTable("PasswordRecoveryTokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.IsUsed).IsRequired();

            entity.HasIndex(e => e.TokenHash).IsUnique();
        });
    }
}
