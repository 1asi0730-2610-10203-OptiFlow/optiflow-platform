using Microsoft.EntityFrameworkCore;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Entities;
using optiflow_platform.Sales.Domain.Model.ValueObjects;

namespace optiflow_platform.Sales.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySalesConfiguration(this ModelBuilder builder)
    {
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

        builder.Entity<SaleItem>().HasKey(i => i.Id);
        builder.Entity<SaleItem>().Property(i => i.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SaleItem>().Property(i => i.SaleId).IsRequired();
        builder.Entity<SaleItem>().Property(i => i.ProductId).IsRequired();
        builder.Entity<SaleItem>().Property(i => i.Quantity).IsRequired();
    }
}
