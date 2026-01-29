using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpectreKickBox.Models
{
    public partial class Membership
    {
        [Key]
        public int MembershipID { get; set; }

        public int UserID { get; set; }
        public int MembershipPlanID { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("UserID")]
        public virtual AppUser AppUser { get; set; } = null!;

        [ForeignKey("MembershipPlanID")]
        public virtual MembershipPlan MembershipPlan { get; set; } = null!;
    }

}