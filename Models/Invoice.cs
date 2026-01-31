using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int AccountId { get; set; }

    public int MembershipPlanId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual MembershipPlan MembershipPlan { get; set; } = null!;
}
