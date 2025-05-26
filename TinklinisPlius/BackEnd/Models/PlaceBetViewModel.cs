using System;
using System.Collections.Generic;
namespace TinklinisPlius.Models
{
    public class PlaceBetViewModel
    {
        public int WagerId { get; set; }
        public int UserId { get; set; } = 1;
        public int Amount { get; set; }
        public string StripeToken { get; set; }
    }
}