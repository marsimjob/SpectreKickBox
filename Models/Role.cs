using System.Collections.Generic;

namespace SpectreKickBox.Models
{
    public partial class Role
    {
        public Role()
        {
            Account = new HashSet<Account>();
        }

        public int RoleID { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<Account> Account { get; set; }
    }
}
