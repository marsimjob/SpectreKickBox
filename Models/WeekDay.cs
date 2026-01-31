using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class WeekDay
{
    public int DayId { get; set; }

    public string DayName { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
