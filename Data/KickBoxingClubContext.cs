using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SpectreKickBox.Models;

namespace SpectreKickBox.Data;

public partial class KickBoxingClubContext : DbContext
{
    public KickBoxingClubContext()
    {
    }

    public KickBoxingClubContext(DbContextOptions<KickBoxingClubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<MembershipPlan> MembershipPlans { get; set; }

    public virtual DbSet<Newsletter> Newsletters { get; set; }

    public virtual DbSet<PriceList> PriceLists { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SessionType> SessionTypes { get; set; }

    public virtual DbSet<VwMemberRole> VwMemberRoles { get; set; }

    public virtual DbSet<VwTotalInvoiceRevenue> VwTotalInvoiceRevenues { get; set; }

    public virtual DbSet<VwTotalSessionsPerTrainer> VwTotalSessionsPerTrainers { get; set; }

    public virtual DbSet<WeekDay> WeekDays { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=KickBoxingClubDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5868A04E314");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_User");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__AppUser__1788CCACE1BFF64B");

            entity.ToTable("AppUser");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__D796AAD5BF6848BD");

            entity.ToTable("Invoice");

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.InvoiceDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MembershipPlanId).HasColumnName("MembershipPlanID");

            entity.HasOne(d => d.Account).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Account");

            entity.HasOne(d => d.MembershipPlan).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.MembershipPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Plan");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("PK__Membersh__92A78599CE0EA54D");

            entity.ToTable("Membership");

            entity.Property(e => e.MembershipId).HasColumnName("MembershipID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MembershipPlanId).HasColumnName("MembershipPlanID");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.MembershipPlan).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.MembershipPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Membership_PlanID");

            entity.HasOne(d => d.User).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Membership_User");
        });

        modelBuilder.Entity<MembershipPlan>(entity =>
        {
            entity.HasKey(e => e.MembershipPlanId).HasName("PK__Membersh__8E444BD6C4681E02");

            entity.ToTable("MembershipPlan");

            entity.Property(e => e.MembershipPlanId).HasColumnName("MembershipPlanID");
            entity.Property(e => e.BillingPeriod)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PriceId).HasColumnName("PriceID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Price).WithMany(p => p.MembershipPlans)
                .HasForeignKey(d => d.PriceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MembershipPlan_PriceList");

            entity.HasOne(d => d.Role).WithMany(p => p.MembershipPlans)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MembershipPlan_Role");
        });

        modelBuilder.Entity<Newsletter>(entity =>
        {
            entity.HasKey(e => e.NewsletterId).HasName("PK__Newslett__34A1DE1D6C158A32");

            entity.ToTable("Newsletter");

            entity.Property(e => e.NewsletterId).HasColumnName("NewsletterID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NewsContent)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.NewsTitle)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NewsType)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PostedByAccountId).HasColumnName("PostedByAccountID");

            entity.HasOne(d => d.PostedByAccount).WithMany(p => p.Newsletters)
                .HasForeignKey(d => d.PostedByAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_PostedBy");
        });

        modelBuilder.Entity<PriceList>(entity =>
        {
            entity.HasKey(e => e.PriceId).HasName("PK__PriceLis__4957584FC10C776C");

            entity.ToTable("PriceList");

            entity.Property(e => e.PriceId).HasColumnName("PriceID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Label)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3AFA5B3143");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Title)
                .HasMaxLength(40)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__Session__C9F49270210CA7AC");

            entity.ToTable("Session");

            entity.Property(e => e.SessionId).HasColumnName("SessionID");
            entity.Property(e => e.DayId).HasColumnName("DayID");
            entity.Property(e => e.Focus)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SessionTypeId).HasColumnName("SessionTypeID");
            entity.Property(e => e.TrainerId).HasColumnName("TrainerID");

            entity.HasOne(d => d.Day).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.DayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Session_DayOfWeek");

            entity.HasOne(d => d.SessionType).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.SessionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Session_SessionTypeID");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Session_TrainerID");
        });

        modelBuilder.Entity<SessionType>(entity =>
        {
            entity.HasKey(e => e.SessionTypeId).HasName("PK__SessionT__D774FFEDF4AA1654");

            entity.ToTable("SessionType");

            entity.Property(e => e.SessionTypeId).HasColumnName("SessionTypeID");
            entity.Property(e => e.TypeTitle)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwMemberRole>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_MemberRole");

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.MemberName)
                .HasMaxLength(61)
                .IsUnicode(false);
            entity.Property(e => e.RoleTitle)
                .HasMaxLength(40)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwTotalInvoiceRevenue>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TotalInvoiceRevenue");

            entity.Property(e => e.NumberOfInvoices).HasColumnName("Number of Invoices");
            entity.Property(e => e.Revenue).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwTotalSessionsPerTrainer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TotalSessionsPerTrainer");

            entity.Property(e => e.TrainerId).HasColumnName("TrainerID");
            entity.Property(e => e.TrainerName)
                .HasMaxLength(61)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WeekDay>(entity =>
        {
            entity.HasKey(e => e.DayId).HasName("PK__WeekDay__BF3DD825BF7A90D7");

            entity.ToTable("WeekDay");

            entity.Property(e => e.DayId)
                .ValueGeneratedNever()
                .HasColumnName("DayID");
            entity.Property(e => e.DayName)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
