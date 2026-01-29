using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpectreKickBox.Models
{
    public partial class Session
    {
        [Key]
        public int SessionID { get; set; }
        public int DayID { get; set; }
        public int TrainerID { get; set; }
        public TimeSpan StartTime { get; set; }
        public string Focus { get; set; } = null!;
        [ForeignKey(nameof(DayID))]
        public virtual WeekDay WeekDay { get; set; } = null!;
        public virtual Account Trainer { get; set; } = null!;
    }
}