using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Domain.Model.Entities;

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;

/// <summary>
///     Model builder extensions
/// </summary>
/// <remarks>
///     This class contains extension methods for the model builder.
///     It includes a method to use the snake case and/or plural naming convention according to an object type.
///     It also pluralizes the table names.
/// </remarks>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies entity configuration for domain models owned directly by the Shared context.
    /// </summary>
    public static void ApplySharedConfiguration(this ModelBuilder builder)
    {
        builder.Entity<SystemNotification>().HasKey(n => n.Id);
        builder.Entity<SystemNotification>().Property(n => n.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SystemNotification>().Property(n => n.RecipientUserId).IsRequired();
        builder.Entity<SystemNotification>().Property(n => n.Category).IsRequired().HasMaxLength(50);
        builder.Entity<SystemNotification>().Property(n => n.Message).IsRequired().HasMaxLength(500);
        builder.Entity<SystemNotification>().Property(n => n.IsRead).IsRequired();
        builder.Entity<SystemNotification>().Property(n => n.CreatedAt).IsRequired();
    }

    /// <summary>
    ///     Use snake case naming convention
    /// </summary>
    /// <remarks>
    ///     This method sets the naming convention for the database tables, columns, keys, foreign keys and indexes to snake
    ///     case.
    /// </remarks>
    public static void UseSnakeCaseNamingConvention(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName)) entity.SetTableName(tableName.ToPlural().ToSnakeCase());

            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName().ToSnakeCase());

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrEmpty(keyName)) key.SetName(keyName.ToSnakeCase());
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var foreignKeyName = foreignKey.GetConstraintName();
                if (!string.IsNullOrEmpty(foreignKeyName)) foreignKey.SetConstraintName(foreignKeyName.ToSnakeCase());
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexDatabaseName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(indexDatabaseName)) index.SetDatabaseName(indexDatabaseName.ToSnakeCase());
            }
        }
    }
}