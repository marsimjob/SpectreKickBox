using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models
{
    public partial class User
    {
        public User()
        {
            Account = new HashSet<Account>();
            Membership = new HashSet<Membership>();
        }

        public int UserID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<Account> Account { get; set; }
        public virtual ICollection<Membership> Membership { get; set; }
    }
}
