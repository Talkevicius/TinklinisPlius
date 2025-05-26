using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace TinklinisPlius.Models
{
    public class PlaceBetViewModel
    {   
    public int WagerId { get; set; }
    public int UserId { get; set; }
    public int Amount { get; set; }
    public string StripeToken { get; set; }

    public int SelectedTeamId { get; set; }
    public List<SelectListItem> Teams { get; set; } = new List<SelectListItem>();
    }
}