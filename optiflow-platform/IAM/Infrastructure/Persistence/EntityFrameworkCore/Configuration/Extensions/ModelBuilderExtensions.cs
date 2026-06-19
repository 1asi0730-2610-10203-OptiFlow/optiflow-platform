using atelier_platform_aplicaciones_web.IAM.Domain.Model.Aggregates;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Entities;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace atelier_platform_aplicaciones_web.IAM.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

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

            entity.Property(e => e.Version).IsConcurrencyToken();
            entity.HasQueryFilter(e => e.DeletedAt == null);

            entity.HasIndex(e => e.Email).IsUnique();
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
