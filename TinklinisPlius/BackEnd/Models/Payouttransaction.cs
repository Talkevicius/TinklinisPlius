using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public partial class Payouttransaction
{
    public int IdPayouttransaction { get; set; }

    public int? Sum { get; set; }

    public DateOnly? Payoutdate { get; set; }

    public int FkBetidBet { get; set; }

    public int FkUseridUser { get; set; }

    public virtual Bet FkBetidBetNavigation { get; set; } = null!;

    public virtual User FkUseridUserNavigation { get; set; } = null!;
}
