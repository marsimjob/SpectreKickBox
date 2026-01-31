using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpectreKickBox.Models
{
    public partial class Account
    {
        [Key]
        public int AccountID { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int UserID { get; set; }
        public int RoleID { get; set; }
        [ForeignKey(nameof(UserID))]
        public virtual AppUser AppUser { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
