using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public partial class Participate
{
    public int FkMatchidMatch { get; set; }

    public int FkMatchfkTournamentidTournament { get; set; }

    public int FkTeamidTeam { get; set; }

    public virtual Match Match { get; set; } = null!;
    public virtual Team Team { get; set; } = null!;
}
