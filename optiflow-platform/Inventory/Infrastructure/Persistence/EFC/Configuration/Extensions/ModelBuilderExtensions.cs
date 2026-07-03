using System;
using Microsoft.EntityFrameworkCore;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.ValueObjects;

namespace optiflow_platform.Inventory.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyInventoryConfiguration(this ModelBuilder builder)
    {
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
        builder.Entity<Product>().Property(p => p.Category).IsRequired().HasMaxLength(255)
            .HasConversion(
                v => v.ToString().ToUpper(),
                v => (EProductCategory)Enum.Parse(typeof(EProductCategory), v, true));
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
    }
}
