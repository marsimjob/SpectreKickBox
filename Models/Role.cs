using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<MembershipPlan> MembershipPlans { get; set; } = new List<MembershipPlan>();
}
