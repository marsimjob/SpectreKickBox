namespace SpectreKickBox.Models
{
    public partial class DayOfWeek
    {
        public DayOfWeek()
        {
            Session = new HashSet<Session>();
        }

        public int DayID { get; set; }
        public string DayName { get; set; } = null!;

        public virtual ICollection<Session> Session { get; set; }
    }
}
