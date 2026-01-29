using System.ComponentModel.DataAnnotations;

namespace SpectreKickBox.Models
{
    public class SessionType
    {
        [Key]
        public int SessionTypeID { get; set; }

        [Required]
        [StringLength(30)]
        public string TypeTitle { get; set; } = string.Empty;

        // Navigation property: One SessionType can have many scheduled Sessions
        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}