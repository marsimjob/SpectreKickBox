using System;
using System.Collections.Generic;

namespace SpectreKickBox.Models;

public partial class SessionType
{
    public int SessionTypeId { get; set; }

    public string TypeTitle { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
