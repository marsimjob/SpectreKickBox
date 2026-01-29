using System;
using System.Collections.Generic;
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

        // 1. ADD: The Foreign Key for SessionType
        public int SessionTypeID { get; set; }

        public TimeSpan StartTime { get; set; }

        // 2. ADD: These were in your SQL but missing from your C# class
        public int Duration { get; set; }
        public int Capacity { get; set; }
        public int SessionWeek { get; set; }

        public string Focus { get; set; } = null!;

        // KEPT: Existing property names
        [ForeignKey(nameof(DayID))]
        public virtual WeekDay WeekDay { get; set; } = null!;

        [ForeignKey(nameof(TrainerID))]
        public virtual Account Trainer { get; set; } = null!;

        // 3. ADD: The missing Navigation Property to fix the error
        [ForeignKey(nameof(SessionTypeID))]
        public virtual SessionType SessionType { get; set; } = null!;
    }
}