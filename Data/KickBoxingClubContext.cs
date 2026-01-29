using Microsoft.EntityFrameworkCore;
using SpectreKickBox.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpectreKickBox.Data
{
    public partial class KickBoxingClubContext : DbContext
    {
        public KickBoxingClubContext() { }

        public KickBoxingClubContext(DbContextOptions<KickBoxingClubContext> options)
            : base(options) { }

        // Standard Tables
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Membership> Membership { get; set; }
        public virtual DbSet<MembershipPlan> MembershipPlan { get; set; }
        public virtual DbSet<PriceList> PriceList { get; set; }
        public virtual DbSet<SessionType> SessionType { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<WeekDay> WeekDay { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<Newsletter> Newsletter { get; set; }
        public virtual DbSet<Role> Role { get; set; }

        // SQL Views (Keyless Entities)
        public virtual DbSet<TotalInvoiceRevenue> vw_TotalInvoiceRevenue { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Fix the MembershipPlan relationship (Resolves PriceListID error)
            modelBuilder.Entity<MembershipPlan>(entity =>
            {
                entity.ToTable("MembershipPlan");

                entity.HasOne(d => d.PriceList)
                      .WithMany()
                      .HasForeignKey(d => d.PriceID); // Forces use of PriceID
            });

            // 2. Map the SQL View
            modelBuilder.Entity<TotalInvoiceRevenue>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_TotalInvoiceRevenue");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    // View Model Class
    public class TotalInvoiceRevenue
    {
        public decimal Revenue { get; set; }

        [Column("Number of Invoices")] // Matches the space in your SQL View
        public int NumberOfInvoices { get; set; }
    }
}