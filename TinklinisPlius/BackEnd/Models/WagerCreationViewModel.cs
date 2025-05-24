using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TinklinisPlius.Models
{
    public class WagerCreationViewModel
    {
        public int SelectedMatchId { get; set; }
        public List<SelectListItem> MatchOptions { get; set; } = new List<SelectListItem>();
    }
}