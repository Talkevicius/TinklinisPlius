using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public partial class Match
{
    public int IdMatch { get; set; }

    public string? Title { get; set; }

    public DateOnly? Date { get; set; }

    public int? Team1score { get; set; }

    public int? Team2score { get; set; }

    public bool? Hashappened { get; set; }

    public int? Placeintournament { get; set; }

    public int FkTeamidTeam { get; set; }

    public int FkTournamentidTournament { get; set; }

    public int? FkMatchidMatch { get; set; }

    public int? FkMatchfkTournamentidTournament { get; set; }

    public int? FkInspectoridUser { get; set; }

    public virtual Inspector? FkInspectoridUserNavigation { get; set; }

    public virtual Team FkTeamidTeamNavigation { get; set; } = null!;

    public virtual Tournament FkTournamentidTournamentNavigation { get; set; } = null!;

    public virtual ICollection<Participate> Participates { get; set; } = new List<Participate>();

    public virtual Wager? Wager { get; set; }
}
