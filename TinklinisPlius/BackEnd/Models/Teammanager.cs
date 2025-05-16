using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public partial class Teammanager
{
    public int IdUser { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual Team? Team { get; set; }
}
