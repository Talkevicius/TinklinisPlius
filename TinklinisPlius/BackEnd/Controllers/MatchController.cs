using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Models;
using TinklinisPlius.Services.Match;

namespace TinklinisPlius.Controllers
{
    public class MatchController : Controller
    {
        private readonly IMatchService _matchService;
        private readonly AppDbContext _context; // Assuming the use of a DbContext for database operations.

        
        public MatchController(IMatchService matchService, AppDbContext context)
        {
            _matchService = matchService;
            _context = context;
        }
        public ActionResult MatchInfoWindow(int id)
        {
            var match = _matchService.GetMatchById(id);
            if (match == null)
                return NotFound();

            return View(match);
        }
    }
}