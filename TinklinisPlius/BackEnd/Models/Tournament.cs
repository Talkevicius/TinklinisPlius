using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public class Tournament
{
    public int IdTournament { get; set; }

    public string? Title { get; set; }

    public DateOnly? Startdate { get; set; }

    public DateOnly? Enddate { get; set; }

    public string? Country { get; set; }

    public int? Teamnr { get; set; }

    public bool? Creationtype { get; set; }

    public bool? Isactive { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<Team> Teams { get; set; } =new List<Team>();
}
