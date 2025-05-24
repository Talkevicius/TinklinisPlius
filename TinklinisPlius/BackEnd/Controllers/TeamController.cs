using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Models;
using TinklinisPlius.Services.Team;

namespace TinklinisPlius.Controllers
{
    public class TeamController: Controller
    {
        private readonly ITeamService _teamService;
        private readonly AppDbContext _context; // Assuming the use of a DbContext for database operations.

        public TeamController(ITeamService teamService,AppDbContext context)
        {
            _teamService = teamService;
            _context = context;
        }

        public ActionResult TeamListPage()
        {
            var teams = _teamService.GetAllTeams();
            if (teams.IsError)
            {
                // Handle error (e.g., show an error page or message)
                return View("Error");
            }
            return View(teams.Value);
        }

        [HttpGet]
        public ActionResult AddTeamWindow()
        {
            return View(new Team());
        }


        [HttpPost]
        public ActionResult AddTeamWindow(Team team)
        {
            if (ModelState.IsValid)
            {
                team.Elo = 1;
                team.Isparticipating = false;
                
                _teamService.CreateTeam(team);
                return RedirectToAction("TeamListPage");
            }
            return View(team);
        }
    }
}