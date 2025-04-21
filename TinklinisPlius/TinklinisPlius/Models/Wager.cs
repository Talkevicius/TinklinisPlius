using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public partial class Wager
{
    public int IdWager { get; set; }

    public int? Chance { get; set; }

    public int? Combinedsum { get; set; }

    public int FkMatchidMatch { get; set; }

    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();

    public virtual Match FkMatchidMatchNavigation { get; set; } = null!;
}
