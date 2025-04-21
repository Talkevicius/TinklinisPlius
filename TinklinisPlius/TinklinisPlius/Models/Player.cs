using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public partial class Player
{
    public int IdPlayer { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string? Position { get; set; }

    public double? Elo { get; set; }

    public int? Points { get; set; }

    public int? Blocks { get; set; }

    public int? Ace { get; set; }

    public int? Mistakes { get; set; }

    public int FkTeamidTeam { get; set; }

    public virtual Team FkTeamidTeamNavigation { get; set; } = null!;
}
