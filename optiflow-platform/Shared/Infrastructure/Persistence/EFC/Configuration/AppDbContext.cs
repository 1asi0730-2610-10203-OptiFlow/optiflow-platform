using optiflow_platform.Analytics.Domain.Model.Aggregates;
using optiflow_platform.Analytics.Domain.Model.Entities;
using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.ValueObjects;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using SubscriptionPaymentStatus = optiflow_platform.Subscription.Domain.Model.ValueObjects.PaymentStatus;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Interceptors;
using Microsoft.EntityFrameworkCore;
using optiflow_platform.PatientCenter.Domain.Model.Entities;
using PaymentId = optiflow_platform.Sales.Domain.Model.ValueObjects.PaymentId;
using SubscriptionAggregate = optiflow_platform.Subscription.Domain.Model.Aggregates.Subscription;
using SubscriptionPayment   = optiflow_platform.Subscription.Domain.Model.Aggregates.Payment;
using SubscriptionBilling   = optiflow_platform.Subscription.Domain.Model.Aggregates.Billing;
using SubscriptionPlan      = optiflow_platform.Subscription.Domain.Model.Aggregates.Plan;


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

        // ── Lab and Orders Bounded Context ────────────────────────────────
        builder.Entity<Laboratory>().HasKey(l => l.Id);
        builder.Entity<Laboratory>().Property(l => l.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Laboratory>().Property(l => l.Name).IsRequired().HasMaxLength(255);
        builder.Entity<Laboratory>().HasIndex(l => l.Name).IsUnique();
        builder.Entity<Laboratory>().OwnsOne(l => l.ContactInfo, ci =>
        {
            ci.WithOwner().HasForeignKey("Id");
            ci.Property(c => c.Phone).HasColumnName("contact_phone").IsRequired().HasMaxLength(50);
            ci.Property(c => c.Email).HasColumnName("contact_email").IsRequired().HasMaxLength(255);
        });

        builder.Entity<WorkOrder>().HasKey(w => w.Id);
        builder.Entity<WorkOrder>().Property(w => w.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<WorkOrder>().Property(w => w.SaleId).IsRequired();
        builder.Entity<WorkOrder>().Property(w => w.RecipeId).IsRequired();
        builder.Entity<WorkOrder>().Property(w => w.LaboratoryId)
            .IsRequired()
            .HasConversion(v => v.Value, v => new LaboratoryId(v));
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

        // ── Analytics Bounded Context ─────────────────────────────────────
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

        // ── Clinical Bounded Context ──────────────────────────────────────
        builder.Entity<Patient>().HasKey(p => p.Id);
        builder.Entity<Patient>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Patient>().Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Entity<Patient>().Property(p => p.LastName).IsRequired().HasMaxLength(100);
        builder.Entity<Patient>().Property(p => p.Dni).IsRequired().HasMaxLength(20);
        builder.Entity<Patient>().HasIndex(p => p.Dni).IsUnique();
        builder.Entity<Patient>().Property(p => p.Phone).HasMaxLength(20);
        builder.Entity<Patient>().Property(p => p.Email).HasMaxLength(255);
        builder.Entity<Patient>().Property(p => p.BirthDate).IsRequired();

        builder.Entity<ClinicalRecord>().HasKey(r => r.Id);
        builder.Entity<ClinicalRecord>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ClinicalRecord>().Property(r => r.PatientId).IsRequired();
        builder.Entity<ClinicalRecord>().HasIndex(r => r.PatientId).IsUnique();
        builder.Entity<ClinicalRecord>()
            .HasOne<Patient>()
            .WithMany()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
        // Ignore the navigation collection — loaded explicitly
        builder.Entity<ClinicalRecord>().Ignore(r => r.Prescriptions);

        builder.Entity<Prescription>().HasKey(p => p.Id);
        builder.Entity<Prescription>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Prescription>().Property(p => p.ClinicalRecordId).IsRequired();
        builder.Entity<Prescription>().Property(p => p.OdSphere).HasColumnType("decimal(5,2)");
        builder.Entity<Prescription>().Property(p => p.OdCylinder).HasColumnType("decimal(5,2)");
        builder.Entity<Prescription>().Property(p => p.OiSphere).HasColumnType("decimal(5,2)");
        builder.Entity<Prescription>().Property(p => p.OiCylinder).HasColumnType("decimal(5,2)");
        builder.Entity<Prescription>().Property(p => p.Addition).HasColumnType("decimal(5,2)");
        builder.Entity<Prescription>().Property(p => p.Notes).HasMaxLength(1000);
        builder.Entity<Prescription>().Property(p => p.DoctorName).IsRequired().HasMaxLength(255);
        builder.Entity<Prescription>().Property(p => p.CreatedAt).IsRequired();
        builder.Entity<Prescription>()
            .HasOne<ClinicalRecord>()
            .WithMany()
            .HasForeignKey(p => p.ClinicalRecordId)
            .OnDelete(DeleteBehavior.Cascade);
        // Sales Bounded Context
        builder.Entity<Sale>().HasKey(s => s.Id);
        builder.Entity<Sale>().Property(s => s.Id)
            .HasConversion(id => id.Value, v => new SaleId(v))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<Sale>().Property(s => s.InvoiceNumber)
            .HasConversion(inv => inv.Value, v => new InvoiceNumber(v))
            .IsRequired()
            .HasMaxLength(50);
        builder.Entity<Sale>().Property(s => s.LabOrderNumber).HasMaxLength(50);
        builder.Entity<Sale>().Property(s => s.PatientId).IsRequired();
        builder.Entity<Sale>().Property(s => s.PatientName).IsRequired().HasMaxLength(255);
        builder.Entity<Sale>().Property(s => s.UserId).IsRequired();
        builder.Entity<Sale>().Property(s => s.UserName).IsRequired().HasMaxLength(255);
        builder.Entity<Sale>().Property(s => s.TotalAmount).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<Sale>().Property(s => s.Advance).HasColumnType("decimal(10,2)");
        builder.Entity<Sale>().Property(s => s.PendingBalance).HasColumnType("decimal(10,2)");
        builder.Entity<Sale>().Property(s => s.DiscountCode).HasMaxLength(50);
        builder.Entity<Sale>().Property(s => s.DiscountAmount).HasColumnType("decimal(10,2)");
        builder.Entity<Sale>().Property(s => s.PaymentMethod).IsRequired().HasMaxLength(50);
        builder.Entity<Sale>().Property(s => s.Status).IsRequired().HasMaxLength(50);
        builder.Entity<Sale>().Property(s => s.CreatedAt).IsRequired().HasMaxLength(50);
        builder.Entity<Sale>().Property(s => s.DeliveredAt).HasMaxLength(50);
        builder.Entity<Sale>().Property(s => s.Notes).HasMaxLength(1000);

        builder.Entity<Payment>().HasKey(p => p.Id);
        builder.Entity<Payment>().Property(p => p.Id)
            .HasConversion(id => id.Value, v => new PaymentId(v))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<Payment>().Property(p => p.SaleId).IsRequired();
        builder.Entity<Payment>().Property(p => p.TotalAmount).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<Payment>().Property(p => p.PaidAmount).HasColumnType("decimal(10,2)");
        builder.Entity<Payment>().Property(p => p.OutstandingBalance).HasColumnType("decimal(10,2)");
        builder.Entity<Payment>().Property(p => p.Status).IsRequired().HasMaxLength(20);
        builder.Entity<Payment>().Property(p => p.Method).IsRequired().HasMaxLength(50);
        builder.Entity<Payment>().Property(p => p.PaidAt).IsRequired().HasMaxLength(50);

        // Inventory Bounded Context
        builder.Entity<Category>().HasKey(c => c.Id);
        builder.Entity<Category>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Category>().Property(c => c.Name).IsRequired().HasMaxLength(255);

        builder.Entity<Supplier>().HasKey(s => s.Id);
        builder.Entity<Supplier>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Supplier>().Property(s => s.Name).IsRequired().HasMaxLength(255);
        builder.Entity<Supplier>().HasIndex(s => s.Name).IsUnique();
        builder.Entity<Supplier>().OwnsOne(s => s.Contact, c =>
        {
            c.WithOwner().HasForeignKey("Id");
            c.Property(x => x.ContactPerson).HasColumnName("contact_person").IsRequired().HasMaxLength(255);
            c.Property(x => x.Phone).HasColumnName("contact_phone").IsRequired().HasMaxLength(50);
            c.Property(x => x.Email).HasColumnName("contact_email").IsRequired().HasMaxLength(255);
        });

        builder.Entity<Product>().HasKey(p => p.Id);
        builder.Entity<Product>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Product>().Property(p => p.CategoryId).IsRequired();
        builder.Entity<Product>().Property(p => p.Category).IsRequired().HasMaxLength(255);
        builder.Entity<Product>().Property(p => p.SupplierId).IsRequired();
        builder.Entity<Product>().Property(p => p.SupplierName).IsRequired().HasMaxLength(255);
        builder.Entity<Product>().Property(p => p.Sku).IsRequired().HasMaxLength(100);
        builder.Entity<Product>().Property(p => p.Name).IsRequired().HasMaxLength(255);
        builder.Entity<Product>().Property(p => p.Brand).HasMaxLength(255);
        builder.Entity<Product>().Property(p => p.Model).HasMaxLength(255);
        builder.Entity<Product>().Property(p => p.Price).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<Product>().Property(p => p.Stock).IsRequired();
        builder.Entity<Product>().Property(p => p.MinimumStockThreshold).IsRequired();
        builder.Entity<Product>().Property(p => p.LastRestockDate).IsRequired().HasMaxLength(50);
        builder.Entity<Product>().HasIndex(p => p.Sku).IsUnique();

        builder.Entity<StockAuditLog>().HasKey(a => a.Id);
        builder.Entity<StockAuditLog>().Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<StockAuditLog>().Property(a => a.ProductId).IsRequired();
        builder.Entity<StockAuditLog>().Property(a => a.ProductName).IsRequired().HasMaxLength(255);
        builder.Entity<StockAuditLog>().Property(a => a.Sku).IsRequired().HasMaxLength(100);
        builder.Entity<StockAuditLog>().Property(a => a.Operation).IsRequired().HasMaxLength(50);
        builder.Entity<StockAuditLog>().Property(a => a.PreviousStock).IsRequired();
        builder.Entity<StockAuditLog>().Property(a => a.Quantity).IsRequired();
        builder.Entity<StockAuditLog>().Property(a => a.NewStock).IsRequired();
        builder.Entity<StockAuditLog>().Property(a => a.Author).IsRequired().HasMaxLength(255);
        builder.Entity<StockAuditLog>().Property(a => a.Date).IsRequired().HasMaxLength(20);
        builder.Entity<StockAuditLog>().Property(a => a.Time).IsRequired().HasMaxLength(20);

        // ── PatientCenter Bounded Context ─────────────────────────────────
        builder.Entity<LensMaterial>().HasKey(l => l.Id);
        builder.Entity<LensMaterial>().Property(l => l.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<LensMaterial>().Property(l => l.FullName).IsRequired().HasMaxLength(255);
        builder.Entity<LensMaterial>().Property(l => l.IndexValue).IsRequired().HasMaxLength(50);
        builder.Entity<LensMaterial>().Property(l => l.BasePrice).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<LensMaterial>().Property(l => l.Description).HasMaxLength(500);

        builder.Entity<PatientNotification>().HasKey(n => n.Id);
        builder.Entity<PatientNotification>().Property(n => n.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<PatientNotification>().Property(n => n.PatientId).IsRequired();
        builder.Entity<PatientNotification>().Property(n => n.Message).IsRequired().HasMaxLength(500);
        builder.Entity<PatientNotification>().Property(n => n.Status).IsRequired().HasMaxLength(20);
        builder.Entity<PatientNotification>().Property(n => n.SentAt).IsRequired();
        
        // ── Subscription Bounded Context ──────────────────────────────────────
        builder.Entity<SubscriptionPlan>().ToTable("subscription_plans");
        builder.Entity<SubscriptionPlan>().HasKey(p => p.Id);
        builder.Entity<SubscriptionPlan>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SubscriptionPlan>().Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Entity<SubscriptionPlan>().Property(p => p.Price).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<SubscriptionPlan>().Property(p => p.Description).HasMaxLength(500);
        builder.Entity<SubscriptionPlan>().Property(p => p.Tier)
            .IsRequired().HasMaxLength(50)
            .HasConversion(v => v.Value, v => new SubscriptionTier(v));
        builder.Entity<SubscriptionPlan>().Ignore(p => p.PlanId);

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

        builder.Entity<SubscriptionPayment>().ToTable("subscription_payments");
        builder.Entity<SubscriptionPayment>().HasKey(p => p.Id);
        builder.Entity<SubscriptionPayment>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SubscriptionPayment>().Property(p => p.SubscriptionId)
            .IsRequired()
            .HasConversion(v => v.Value, v => new SubscriptionId(v));
        builder.Entity<SubscriptionPayment>().Property(p => p.Amount).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<SubscriptionPayment>().Property(p => p.PaymentMethod).IsRequired().HasMaxLength(50);
        builder.Entity<SubscriptionPayment>().Property(p => p.Status)
            .IsRequired().HasMaxLength(50)
            .HasConversion(v => v.Value, v => new SubscriptionPaymentStatus(v));

        builder.Entity<SubscriptionBilling>().ToTable("subscription_billings");
        builder.Entity<SubscriptionBilling>().HasKey(b => b.Id);
        builder.Entity<SubscriptionBilling>().Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SubscriptionBilling>().Property(b => b.SubscriptionId)
            .IsRequired()
            .HasConversion(v => v.Value, v => new SubscriptionId(v));
        builder.Entity<SubscriptionBilling>().Property(b => b.BillingStatus).IsRequired().HasMaxLength(50);
        builder.Entity<SubscriptionBilling>().Property(b => b.AutoRenew).IsRequired();

        builder.UseSnakeCaseNamingConvention();
    }
}
