using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class VwTotalSessionsPerTrainer
{
    public int TrainerId { get; set; }

    public string TrainerName { get; set; } = null!;

    public int? TotalSessions { get; set; }
}
