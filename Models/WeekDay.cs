using System.ComponentModel.DataAnnotations;

namespace SpectreKickBox.Models
{
    public partial class WeekDay
    {
        public WeekDay()
        {
            Session = new HashSet<Session>();
        }
        [Key]
        public int DayID { get; set; }
        public string DayName { get; set; } = null!;

        public virtual ICollection<Session> Session { get; set; }
    }
}
