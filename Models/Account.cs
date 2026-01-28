using System;

namespace SpectreKickBox.Models
{
    public partial class Account
    {
        public int AccountID { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int UserID { get; set; }
        public int RoleID { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
