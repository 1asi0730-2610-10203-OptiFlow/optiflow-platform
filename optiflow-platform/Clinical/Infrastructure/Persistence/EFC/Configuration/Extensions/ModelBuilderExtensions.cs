using Microsoft.EntityFrameworkCore;
using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Model.Entities;

namespace optiflow_platform.Clinical.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyClinicalConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Patient>().HasKey(p => p.Id);
        builder.Entity<Patient>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Patient>().Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Entity<Patient>().Property(p => p.LastName).IsRequired().HasMaxLength(100);
        builder.Entity<Patient>().Property(p => p.Dni).IsRequired().HasMaxLength(20);
        builder.Entity<Patient>().HasIndex(p => p.Dni).IsUnique();
        builder.Entity<Patient>().Property(p => p.Phone).HasMaxLength(20);
        builder.Entity<Patient>().Property(p => p.Email).HasMaxLength(255);
        builder.Entity<Patient>().Property(p => p.BirthDate).IsRequired()
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v)
            );
        builder.Entity<ClinicalRecord>().HasKey(r => r.Id);
        builder.Entity<ClinicalRecord>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ClinicalRecord>().Property(r => r.PatientId).IsRequired();
        builder.Entity<ClinicalRecord>().HasIndex(r => r.PatientId).IsUnique();
        builder.Entity<ClinicalRecord>()
            .HasOne<Patient>()
            .WithMany()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
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
    }
}
