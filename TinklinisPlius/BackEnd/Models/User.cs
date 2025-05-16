using System;
using System.Collections.Generic;

namespace TinklinisPlius.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Cardnumber { get; set; }

    public DateOnly? Cardexpirationdate { get; set; }

    public int? Cardcvc { get; set; }

    public double? Riskfactor { get; set; }

    public double? Lossrate { get; set; }

    public double? Betvariance { get; set; }

    public double? Highoddsfrequency { get; set; }

    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();

    public virtual Inspector? Inspector { get; set; }

    public virtual ICollection<Payouttransaction> Payouttransactions { get; set; } = new List<Payouttransaction>();

    public virtual Teammanager? Teammanager { get; set; }
}
