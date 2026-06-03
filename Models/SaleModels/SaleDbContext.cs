using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Chap10.Models.SaleModels;

public partial class SaleDbContext : DbContext
{
    public SaleDbContext()
    {
    }

    public SaleDbContext(DbContextOptions<SaleDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<FinancingContract> FinancingContracts { get; set; }

    public virtual DbSet<SalesDocument> SalesDocuments { get; set; }

    public virtual DbSet<SalesTransaction> SalesTransactions { get; set; }

    public virtual DbSet<WarrantyRegistration> WarrantyRegistrations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=SalesSystem;uid=root;pwd=123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.36-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");

            entity.ToTable("Customer");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CustomerType).HasMaxLength(100);
            entity.Property(e => e.DriverLicenseNumber).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.LoyaltyTier).HasMaxLength(50);
            entity.Property(e => e.NationalId).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<FinancingContract>(entity =>
        {
            entity.HasKey(e => e.FinancingContractId).HasName("PRIMARY");

            entity.ToTable("FinancingContract");

            entity.HasIndex(e => e.TransactionId, "FK_FinancingContract_Transaction");

            entity.Property(e => e.ApprovalStatus).HasMaxLength(100);
            entity.Property(e => e.ContractEndDate).HasColumnType("datetime");
            entity.Property(e => e.ContractStartDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FinancingProvider).HasMaxLength(255);
            entity.Property(e => e.InterestRate).HasPrecision(5, 2);
            entity.Property(e => e.LoanAmount).HasPrecision(15, 2);
            entity.Property(e => e.MonthlyPayment).HasPrecision(15, 2);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Transaction).WithMany(p => p.FinancingContracts)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK_FinancingContract_Transaction");
        });

        modelBuilder.Entity<SalesDocument>(entity =>
        {
            entity.HasKey(e => e.SalesDocumentId).HasName("PRIMARY");

            entity.ToTable("SalesDocument");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DocumentNumber).HasMaxLength(100);
            entity.Property(e => e.DocumentTitle).HasMaxLength(255);
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.FileFormat).HasMaxLength(50);
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.GeneratedDate).HasColumnType("datetime");
            entity.Property(e => e.SourceSystem).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<SalesTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PRIMARY");

            entity.ToTable("SalesTransaction");

            entity.HasIndex(e => e.CustomerId, "FK_SalesTransaction_Customer");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountAmount).HasPrecision(15, 2);
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            entity.Property(e => e.SalesDate).HasColumnType("datetime");
            entity.Property(e => e.SellingPrice).HasPrecision(15, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(15, 2);
            entity.Property(e => e.TransactionStatus).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesTransactions)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_SalesTransaction_Customer");
        });

        modelBuilder.Entity<WarrantyRegistration>(entity =>
        {
            entity.HasKey(e => e.WarrantyId).HasName("PRIMARY");

            entity.ToTable("WarrantyRegistration");

            entity.Property(e => e.CoverageDetails).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.WarrantyProvider).HasMaxLength(255);
            entity.Property(e => e.WarrantyType).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
