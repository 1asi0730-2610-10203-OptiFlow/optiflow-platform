using optiflow_platform.Analytics.Domain.Model.Aggregates;
using optiflow_platform.Analytics.Domain.Model.Entities;
using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Entities;
using optiflow_platform.Sales.Domain.Model.Aggregates;
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

        // ── Lab and Orders Bounded Context ────────────────────────────────
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
        builder.Entity<Sale>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Sale>().Property(s => s.ClientName).IsRequired().HasMaxLength(255);
        builder.Entity<Sale>().Property(s => s.TotalAmount).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<Sale>().Property(s => s.QuotaAmount).HasColumnType("decimal(10,2)");
        builder.Entity<Sale>().Property(s => s.DiscountPercentage).HasColumnType("decimal(5,2)");
        builder.Entity<Sale>().Property(s => s.Status).IsRequired().HasMaxLength(50);
        builder.Entity<Sale>().Property(s => s.SaleDate).IsRequired().HasMaxLength(50);

        builder.Entity<Payment>().HasKey(p => p.Id);
        builder.Entity<Payment>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Payment>().Property(p => p.SaleId).IsRequired();
        builder.Entity<Payment>().Property(p => p.TotalAmount).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<Payment>().Property(p => p.PaidAmount).HasColumnType("decimal(10,2)");
        builder.Entity<Payment>().Property(p => p.OutstandingBalance).HasColumnType("decimal(10,2)");
        builder.Entity<Payment>().Property(p => p.Status).IsRequired().HasMaxLength(20);

        // Inventory Bounded Context
        builder.Entity<Category>().HasKey(c => c.Id);
        builder.Entity<Category>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Category>().Property(c => c.Name).IsRequired().HasMaxLength(255);

        builder.Entity<Supplier>().HasKey(s => s.Id);
        builder.Entity<Supplier>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Supplier>().Property(s => s.Name).IsRequired().HasMaxLength(255);
        builder.Entity<Supplier>().Property(s => s.ContactPerson).HasMaxLength(255);
        builder.Entity<Supplier>().Property(s => s.Phone).HasMaxLength(50);
        builder.Entity<Supplier>().Property(s => s.Email).HasMaxLength(255);

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

        builder.UseSnakeCaseNamingConvention();
    }
}
