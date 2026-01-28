using SpectreKickBox.Models; // Om dina entity‑klasser ligger i Models
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;


namespace SpectreKickBox.Data
{
    public partial class KickBoxingClubContext : DbContext
    {
        public KickBoxingClubContext()
        {
        }

        public KickBoxingClubContext(DbContextOptions<KickBoxingClubContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Membership> Membership { get; set; }
        public virtual DbSet<MembershipPlan> MembershipPlan { get; set; }
        public virtual DbSet<PriceList> PriceList { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<SpectreKickBox.Models.DayOfWeek> DayOfWeek { get; set; }
        public virtual DbSet<Newsletter> Newsletter { get; set; }
        public virtual DbSet<Role> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Här definieras relationer, constraints osv.
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
