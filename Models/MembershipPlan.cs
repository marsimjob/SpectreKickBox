using System.Collections.Generic;

namespace SpectreKickBox.Models
{
    public partial class MembershipPlan
    {
        public MembershipPlan()
        {
            Membership = new HashSet<Membership>();
        }

        public int MembershipPlanID { get; set; }
        public string BillingPeriod { get; set; } = null!;
        public int PriceListID { get; set; }

        public virtual PriceList PriceList { get; set; } = null!;
        public virtual ICollection<Membership> Membership { get; set; }
    }
}
