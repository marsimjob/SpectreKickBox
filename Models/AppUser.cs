using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SpectreKickBox.Models
{
    public partial class AppUser
    {
        public AppUser()
        {
            Account = new HashSet<Account>();
            Membership = new HashSet<Membership>();
        }
        [Key]
        public int UserID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<Account> Account { get; set; }
        public virtual ICollection<Membership> Membership { get; set; }
    }
}
