using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class AppUser
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}
