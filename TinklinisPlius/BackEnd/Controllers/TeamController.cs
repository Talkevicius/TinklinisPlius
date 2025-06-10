using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
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
        public ActionResult AddTeamWindow(Team team)
        {
            var allTeamsResult = _teamService.GetAllTeams();

            if (allTeamsResult.IsError)
            {
                ModelState.AddModelError("", "Failed to retrieve teams list. Please try again.");
                return View(team);
            }

            if (allTeamsResult.Value.Any(t => t.Name.Equals(team.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Name", "Team already exists");
                return View(team);
            }
            if (ModelState.IsValid)
            {
                team.Isparticipating = false;

                var addResult = _teamService.AddTeam(team);

                if (addResult.IsError)
                {
                    ModelState.AddModelError("", "Failed to add team. Please try again.");
                    return View(team);
                }

                _teamService.SetEloTo1(team, 1);

                _teamService.Save();

                return RedirectToAction("TeamListPage");
            }

            return View(team);
        }

           

        /*[HttpPost]
        public ActionResult AddTeamWindow(Team team)
        {
            // 1. AddTeamWindow() - inicijuojamas procesas (jau įvyksta per [HttpPost] kvietimą)

            // 2. Tikriname ar komanda jau egzistuoja (atitinka žingsnį 8)
            if (_teamService.TeamExistsByName(team.Name))
            {
                // 8. error - grąžiname validacijos klaidą
                ModelState.AddModelError("Name", "Team already exists");
                return View(team);
            }

            // 4. team() - dirbame su komandos objektu
            // 5. Eksplicitiai iškviečiame addTeam() operaciją
            _teamService.AddTeam(team);  // Vietoj CreateTeam

            // 10. setEloTo(1) - aiškiai išskirtas Elo nustatymas
            _teamService.SetEloTo(team, 1);

            // 11-12. success - nukreipiame į sąrašo puslapį
            return RedirectToAction("TeamListPage");
        }
        */

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
        public IActionResult EditTeam(Team team)
        {
            var validationResult = _teamService.ValidateData(team);
            if (validationResult.IsError)
            {
                TempData["Message"] = validationResult.FirstError.Description;
                return RedirectToAction("TeamListPage");
            }

            var getTeamResult = _teamService.GetTeam(team.IdTeam);
            if (getTeamResult.IsError)
            {
                return NotFound();
            }
            var existingTeam = getTeamResult.Value;

            var selectTeamResult = _teamService.SelectTeam(team.Name, team.IdTeam);
            if (selectTeamResult.IsError)
            {
                // Jei reikia, gali čia kažką daryti su klaida, bet dažniausiai tiesiog tęs...
            }
            else if (selectTeamResult.Value)
            {
                ModelState.AddModelError("Name", "Team with this name already exists.");
                return View(team);
            }

            if (ModelState.IsValid)
            {
                existingTeam.Name = team.Name;
                existingTeam.Trainer = team.Trainer;
                existingTeam.Country = team.Country;

                _teamService.Save();

                TempData["Message"] = "Team updated successfully.";
                return RedirectToAction("TeamListPage");
            }

            return View(team);
        }


        /*[HttpPost]
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
        */
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

        [HttpGet]
        public IActionResult PlayersByTeam(int id)
        {
            var team = _context.Teams
                .Include(t => t.Players)
                .FirstOrDefault(t => t.IdTeam == id);

            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }




    }
}