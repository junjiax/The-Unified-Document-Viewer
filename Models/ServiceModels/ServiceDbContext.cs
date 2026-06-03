using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Chap10.Models.ServiceModels;

public partial class ServiceDbContext : DbContext
{
    public ServiceDbContext()
    {
    }

    public ServiceDbContext(DbContextOptions<ServiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DiagnosticReport> DiagnosticReports { get; set; }

    public virtual DbSet<ServiceDocument> ServiceDocuments { get; set; }

    public virtual DbSet<ServiceRecord> ServiceRecords { get; set; }

    public virtual DbSet<Technician> Technicians { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=ServicesSystem;uid=root;pwd=123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.36-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<DiagnosticReport>(entity =>
        {
            entity.HasKey(e => e.DiagnosticReportId).HasName("PRIMARY");

            entity.ToTable("DiagnosticReport");

            entity.HasIndex(e => e.ServiceRecordId, "FK_DiagnosticReport_ServiceRecord");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DiagnosticCode).HasMaxLength(100);
            entity.Property(e => e.GeneratedDate).HasColumnType("datetime");
            entity.Property(e => e.ProblemDescription).HasColumnType("text");
            entity.Property(e => e.RecommendedAction).HasColumnType("text");
            entity.Property(e => e.ResolutionStatus).HasMaxLength(50);
            entity.Property(e => e.SeverityLevel).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ServiceRecord).WithMany(p => p.DiagnosticReports)
                .HasForeignKey(d => d.ServiceRecordId)
                .HasConstraintName("FK_DiagnosticReport_ServiceRecord");
        });

        modelBuilder.Entity<ServiceDocument>(entity =>
        {
            entity.HasKey(e => e.ServiceDocumentId).HasName("PRIMARY");

            entity.ToTable("ServiceDocument");

            entity.HasIndex(e => e.ServiceRecordId, "FK_ServiceDocument_ServiceRecord");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DocumentTitle).HasMaxLength(255);
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.FileFormat).HasMaxLength(50);
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.GeneratedDate).HasColumnType("datetime");
            entity.Property(e => e.SourceSystem).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ServiceRecord).WithMany(p => p.ServiceDocuments)
                .HasForeignKey(d => d.ServiceRecordId)
                .HasConstraintName("FK_ServiceDocument_ServiceRecord");
        });

        modelBuilder.Entity<ServiceRecord>(entity =>
        {
            entity.HasKey(e => e.ServiceRecordId).HasName("PRIMARY");

            entity.ToTable("ServiceRecord");

            entity.HasIndex(e => e.TechnicianId, "FK_ServiceRecord_Technician");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CustomerComplaint).HasColumnType("text");
            entity.Property(e => e.LaborCost).HasPrecision(10, 2);
            entity.Property(e => e.PartsCost).HasPrecision(10, 2);
            entity.Property(e => e.ServiceEndTime).HasColumnType("datetime");
            entity.Property(e => e.ServiceStartTime).HasColumnType("datetime");
            entity.Property(e => e.ServiceStatus).HasMaxLength(100);
            entity.Property(e => e.ServiceType).HasMaxLength(100);
            entity.Property(e => e.TechnicianNotes).HasColumnType("text");
            entity.Property(e => e.TotalCost).HasPrecision(10, 2);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Technician).WithMany(p => p.ServiceRecords)
                .HasForeignKey(d => d.TechnicianId)
                .HasConstraintName("FK_ServiceRecord_Technician");
        });

        modelBuilder.Entity<Technician>(entity =>
        {
            entity.HasKey(e => e.TechnicianId).HasName("PRIMARY");

            entity.ToTable("Technician");

            entity.Property(e => e.CertificationLevel).HasMaxLength(100);
            entity.Property(e => e.ContactNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.EmploymentStatus).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Specialization).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
