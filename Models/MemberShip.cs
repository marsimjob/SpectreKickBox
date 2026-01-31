using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class Membership
{
    public int MembershipId { get; set; }

    public int UserId { get; set; }

    public bool IsActive { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int MembershipPlanId { get; set; }

    public virtual MembershipPlan MembershipPlan { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}
