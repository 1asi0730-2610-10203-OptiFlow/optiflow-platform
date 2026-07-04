using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace optiflow_platform.Clinical.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    // MySql.Data no soporta DateOnly nativo — necesita conversión explícita DateTime ↔ DateOnly
    private static readonly ValueConverter<DateOnly, DateTime> DateOnlyConverter = new(
        dateOnly  => dateOnly.ToDateTime(TimeOnly.MinValue),   // DateOnly → DateTime (guardar)
        dateTime  => DateOnly.FromDateTime(dateTime)            // DateTime → DateOnly (leer)
    );

    public static void ApplyClinicalConfiguration(this ModelBuilder builder, AppDbContext dbContext)
    {
        // ── Patient ───────────────────────────────────────────────────────
        builder.Entity<Patient>().HasKey(p => p.Id);
        builder.Entity<Patient>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Patient>().Property(p => p.AccountId).IsRequired();
        builder.Entity<Patient>().HasQueryFilter(p => p.AccountId == dbContext.CurrentAccountId);
        builder.Entity<Patient>().Ignore(p => p.PatientId);          // computed alias, no column
        builder.Entity<Patient>().Property(p => p.CustomerUuid).IsRequired().HasMaxLength(100);
        builder.Entity<Patient>().HasIndex(p => p.CustomerUuid).IsUnique();
        builder.Entity<Patient>().Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Entity<Patient>().Property(p => p.LastName).IsRequired().HasMaxLength(100);
        builder.Entity<Patient>().Property(p => p.Dni).IsRequired().HasMaxLength(20);
        builder.Entity<Patient>().HasIndex(p => new { p.AccountId, p.Dni }).IsUnique();
        builder.Entity<Patient>().Property(p => p.Phone).HasMaxLength(20);
        builder.Entity<Patient>().Property(p => p.Email).HasMaxLength(255);
        builder.Entity<Patient>().Property(p => p.BirthDate).IsRequired()
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v)
            );
        builder.Entity<ClinicalRecord>().HasKey(r => r.Id);
        builder.Entity<ClinicalRecord>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ClinicalRecord>().Property(r => r.AccountId).IsRequired();
        builder.Entity<ClinicalRecord>().HasQueryFilter(r => r.AccountId == dbContext.CurrentAccountId);
        builder.Entity<ClinicalRecord>().Ignore(r => r.RecordId);    // computed alias, no column
        builder.Entity<ClinicalRecord>().Property(r => r.ClinicalRecordUuid).IsRequired().HasMaxLength(100);
        builder.Entity<ClinicalRecord>().HasIndex(r => r.ClinicalRecordUuid).IsUnique();
        builder.Entity<ClinicalRecord>().Property(r => r.PatientId).IsRequired();
        builder.Entity<ClinicalRecord>().HasIndex(r => r.PatientId).IsUnique();
        builder.Entity<ClinicalRecord>()
            .HasOne<Patient>()
            .WithMany()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ClinicalRecord>().Ignore(r => r.Prescriptions);

        // ── Prescription ──────────────────────────────────────────────────
        builder.Entity<Prescription>().HasKey(p => p.Id);
        builder.Entity<Prescription>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Prescription>().Property(p => p.AccountId).IsRequired();
        builder.Entity<Prescription>().HasQueryFilter(p => p.AccountId == dbContext.CurrentAccountId);
        builder.Entity<Prescription>().Ignore(p => p.PrescriptionId); // computed alias, no column
        builder.Entity<Prescription>().Property(p => p.PrescriptionUuid).IsRequired().HasMaxLength(100);
        builder.Entity<Prescription>().HasIndex(p => p.PrescriptionUuid).IsUnique();
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
    }
}