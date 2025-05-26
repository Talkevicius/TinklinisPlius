using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TinklinisPlius.Models;

public class Tournament
{
    public int IdTournament { get; set; }
    [Required(ErrorMessage = "Title is required")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateOnly? Startdate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateOnly? Enddate { get; set; }

    [Required(ErrorMessage = "Country is required")]
    public string? Country { get; set; }

    public int? Teamnr { get; set; }

    public bool? Creationtype { get; set; }

    public bool? Isactive { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<Team> Teams { get; set; } =new List<Team>();
}
