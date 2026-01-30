using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpectreKickBox.Models
{
    public partial class MembershipPlan
    {
        [Key]
        public int MembershipPlanID { get; set; }

        public string BillingPeriod { get; set; } = null!;

        public int PriceID { get; set; }

        public int RoleID { get; set; }

        [ForeignKey(nameof(PriceID))]
        public virtual PriceList PriceList { get; set; } = null!;

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; } = null!;

        public virtual ICollection<Membership> Memberships { get; set; } = new HashSet<Membership>();
    }
}
