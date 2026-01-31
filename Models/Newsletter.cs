using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class Newsletter
{
    public int NewsletterId { get; set; }

    public int PostedByAccountId { get; set; }

    public string NewsTitle { get; set; } = null!;

    public string NewsContent { get; set; } = null!;

    public int PostWeek { get; set; }

    public int PostYear { get; set; }

    public string NewsType { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Account PostedByAccount { get; set; } = null!;
}
