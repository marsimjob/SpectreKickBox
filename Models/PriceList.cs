using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class PriceList
{
    public int PriceId { get; set; }

    public string Label { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual ICollection<MembershipPlan> MembershipPlans { get; set; } = new List<MembershipPlan>();
}
