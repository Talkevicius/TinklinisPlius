using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Models;
using TinklinisPlius.Services.Team;

namespace TinklinisPlius.Controllers
{
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly AppDbContext _context; // Assuming the use of a DbContext for database operations.

        public TeamController(ITeamService teamService, AppDbContext context)
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
        [HttpPost]
        public ActionResult AddTeamWindow(Team team)
        {
            if (_teamService.TeamExistsByName(team.Name))
            {
                ModelState.AddModelError("Name", "Team already exists");
                return View(team);
            }

            if (ModelState.IsValid)
            {
                team.Elo = 1;
                team.Isparticipating = false;

                _teamService.CreateTeam(team);
                return RedirectToAction("TeamListPage");
            }

            return View(team);
        }

        [HttpPost]
        public IActionResult DeleteTeam(int id, string confirm)
        {
            if (confirm == "no")
            {
                TempData["Message"] = "Successfully canceled.";
                return RedirectToAction("TeamListPage");
            }

            var team = _context.Teams.FirstOrDefault(t => t.IdTeam == id);
            if (team == null)
            {
                return NotFound();
            }

            _context.Teams.Remove(team);
            _context.SaveChanges();

            TempData["Message"] = "Successfully deleted.";
            return RedirectToAction("TeamListPage");
        }

        [HttpGet]
        public IActionResult EditTeam(int id)
        {
            var team = _context.Teams.FirstOrDefault(t => t.IdTeam == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult EditTeam(Team team)
        {
            // Patikrina ar visi reikšmingi laukai tušti (ignoruoja tarpus)
            bool allEmpty = string.IsNullOrWhiteSpace(team.Name)
                && string.IsNullOrWhiteSpace(team.Trainer)
                && string.IsNullOrWhiteSpace(team.Country);

            if (allEmpty)
            {
                TempData["Message"] = "No data entered. Changes were canceled.";
                return RedirectToAction("TeamListPage");
            }

            // Patikrina ar kitas įrašas jau turi tokį patį pavadinimą
            if (_context.Teams.Any(t => t.Name.ToLower() == team.Name.ToLower() && t.IdTeam != team.IdTeam))
            {
                ModelState.AddModelError("Name", "Team with this name already exists.");
                return View(team);
            }

            if (ModelState.IsValid)
            {
                var existing = _context.Teams.FirstOrDefault(t => t.IdTeam == team.IdTeam);
                if (existing == null)
                {
                    return NotFound();
                }

                existing.Name = team.Name;
                existing.Trainer = team.Trainer;
                existing.Country = team.Country;

                _context.SaveChanges();

                TempData["Message"] = "Team updated successfully.";
                return RedirectToAction("TeamListPage");
            }

            return View(team);
        }

        [HttpGet]
        public IActionResult TeamElo()
        {
            var teamsResult = _teamService.GetAllTeams();
            if (teamsResult.IsError)
            {
                return View("Error");
            }

            var teams = teamsResult.Value;
            return View(teams);
        }




    }
}