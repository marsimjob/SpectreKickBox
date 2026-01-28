using System;

namespace SpectreKickBox.Models
{
    public partial class Membership
    {
        public int MembershipID { get; set; }
        public int UserID { get; set; }
        public int MembershipPlanID { get; set; }
        public bool IsActive { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual MembershipPlan MembershipPlan { get; set; } = null!;
    }
}
