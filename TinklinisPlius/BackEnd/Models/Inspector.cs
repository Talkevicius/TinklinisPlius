using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public partial class Inspector
{
    public int IdUser { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}
