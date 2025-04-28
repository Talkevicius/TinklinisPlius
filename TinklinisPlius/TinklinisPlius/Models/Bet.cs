using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public partial class Bet
{
    public int IdBet { get; set; }

    public int? Moneybetted { get; set; }

    public int? Chanceofwinning { get; set; }

    public int? Payout { get; set; }

    public DateOnly? Betdate { get; set; }

    public bool? Bettype { get; set; }

    public bool? Result { get; set; }

    public int FkWageridWager { get; set; }

    public int FkUseridUser { get; set; }

    public virtual User FkUseridUserNavigation { get; set; } = null!;

    public virtual Wager FkWageridWagerNavigation { get; set; } = null!;

    public virtual Payouttransaction? Payouttransaction { get; set; }
}
