using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class MembershipPlan
{
    public int MembershipPlanId { get; set; }

    public int RoleId { get; set; }

    public int PriceId { get; set; }

    public string BillingPeriod { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual PriceList Price { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
