using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class VwMemberRole
{
    public int AccountId { get; set; }

    public string MemberName { get; set; } = null!;

    public string RoleTitle { get; set; } = null!;
}
