using Microsoft.EntityFrameworkCore;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using SubscriptionAggregate = optiflow_platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace optiflow_platform.Subscription.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySubscriptionConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Plan>().ToTable("subscription_plans");
        builder.Entity<Plan>().HasKey(p => p.Id);
        builder.Entity<Plan>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Plan>().Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Entity<Plan>().Property(p => p.Price).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<Plan>().Property(p => p.Description).HasMaxLength(500);
        builder.Entity<Plan>().Property(p => p.Tier)
            .IsRequired().HasMaxLength(50)
            .HasConversion(v => v.Value, v => new SubscriptionTier(v));
        builder.Entity<Plan>().Ignore(p => p.PlanId);

        builder.Entity<SubscriptionAggregate>().ToTable("subscriptions");
        builder.Entity<SubscriptionAggregate>().HasKey(s => s.Id);
        builder.Entity<SubscriptionAggregate>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SubscriptionAggregate>().Property(s => s.AdminId).IsRequired();
        builder.Entity<SubscriptionAggregate>().Property(s => s.PlanId)
            .IsRequired()
            .HasConversion(v => v.Value, v => new PlanId(v));
        builder.Entity<SubscriptionAggregate>().Property(s => s.Tier)
            .IsRequired().HasMaxLength(50)
            .HasConversion(v => v.Value, v => new SubscriptionTier(v));
        builder.Entity<SubscriptionAggregate>().Property(s => s.Amount).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<SubscriptionAggregate>().Property(s => s.PaymentMethod).IsRequired().HasMaxLength(50);
        builder.Entity<SubscriptionAggregate>().Property(s => s.Status)
            .IsRequired().HasMaxLength(50)
            .HasConversion(v => v.Value, v => new SubscriptionStatus(v));

        builder.Entity<Payment>().ToTable("subscription_payments");
        builder.Entity<Payment>().HasKey(p => p.Id);
        builder.Entity<Payment>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Payment>().Property(p => p.SubscriptionId)
            .IsRequired()
            .HasConversion(v => v.Value, v => new SubscriptionId(v));
        builder.Entity<Payment>().Property(p => p.Amount).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<Payment>().Property(p => p.PaymentMethod).IsRequired().HasMaxLength(50);
        builder.Entity<Payment>().Property(p => p.Status)
            .IsRequired().HasMaxLength(50)
            .HasConversion(v => v.Value, v => new PaymentStatus(v));

        builder.Entity<Billing>().ToTable("subscription_billings");
        builder.Entity<Billing>().HasKey(b => b.Id);
        builder.Entity<Billing>().Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Billing>().Property(b => b.SubscriptionId)
            .IsRequired()
            .HasConversion(v => v.Value, v => new SubscriptionId(v));
        builder.Entity<Billing>().Property(b => b.BillingStatus).IsRequired().HasMaxLength(50);
        builder.Entity<Billing>().Property(b => b.AutoRenew).IsRequired();
    }
}
