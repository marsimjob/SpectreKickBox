using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Newsletter> Newsletters { get; set; } = new List<Newsletter>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual AppUser User { get; set; } = null!;
}
