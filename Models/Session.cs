using System;

namespace SpectreKickBox.Models
{
    public partial class Session
    {
        public int SessionID { get; set; }
        public int DayID { get; set; }
        public int TrainerID { get; set; }
        public TimeSpan StartTime { get; set; }
        public string Description { get; set; } = null!;

        public virtual DayOfWeek DayOfWeek { get; set; } = null!;
        public virtual Account Trainer { get; set; } = null!;
    }
}