using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public partial class Team
{
    public int IdTeam { get; set; }

    public string? Name { get; set; }

    public string? Country { get; set; }

    public string? Trainer { get; set; }

    public int? Elo { get; set; }

    public bool? Isparticipating { get; set; }

    public int? FkTeammanageridUser { get; set; }

    public int? FkTournamentidTournament { get; set; }

    public virtual Teammanager? FkTeammanageridUserNavigation { get; set; }

    public virtual Tournament? FkTournamentidTournamentNavigation { get; set; }

    public virtual Match? Match { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
