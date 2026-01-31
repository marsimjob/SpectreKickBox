using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class Session
{
    public int SessionId { get; set; }

    public int TrainerId { get; set; }

    public int SessionTypeId { get; set; }

    public TimeOnly StartTime { get; set; }

    public int Duration { get; set; }

    public int Capacity { get; set; }

    public string? Focus { get; set; }

    public int DayId { get; set; }

    public int SessionWeek { get; set; }

    public virtual WeekDay Day { get; set; } = null!;

    public virtual SessionType SessionType { get; set; } = null!;

    public virtual Account Trainer { get; set; } = null!;
}
