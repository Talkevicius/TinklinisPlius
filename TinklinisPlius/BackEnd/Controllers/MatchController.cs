using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Models;
using TinklinisPlius.Services.Match;
using TinklinisPlius.Services.Participate;
using TinklinisPlius.Services.Tournament;

namespace TinklinisPlius.Controllers
{
    public class MatchController : Controller
    {
        private readonly IMatchService _matchService;
        private readonly IParticipateService _participateService;
        private readonly ITournamentService _tournamentService;
        private readonly AppDbContext _context; // Assuming the use of a DbContext for database operations.


        public MatchController(IMatchService matchService, IParticipateService participateService,
            ITournamentService tournamentService, AppDbContext context)
        {
            _matchService = matchService;
            _participateService = participateService;
            _tournamentService = tournamentService;
            _context = context;
        }

        public ActionResult MatchInfoWindow(int id)
        {
            var match = _matchService.GetMatchById(id);
            if (match == null)
                return NotFound();
            var participateResult = _participateService.GetParticipatesByMatchId(id);

            match.Participates = participateResult.Value;
            return View(match);
        }
        [HttpPost]
        public IActionResult SubmitResults(int matchId, int team1Score, int team2Score)
        {
            var match = _matchService.GetMatchById(matchId);
            if (match == null || match.Hashappened == true)
            {
                return BadRequest("Invalid or already finished match.");
            }

            var participates = _participateService.GetParticipatesByMatchId(matchId);

            if (participates.IsError)
            {
                return BadRequest("Could not retrieve participates.");
            }

            var participateList = participates.Value;

            if (participateList.Count < 2)
            {
                return BadRequest("Match must have two teams.");
            }

            if (team1Score == team2Score)
            {
                // Add a ModelState error
                ModelState.AddModelError("", "A draw is not allowed. Please enter a winning score.");

                // Reload participates to pass to view
                match.Participates = participateList;

                // Return the same view with the model and validation error
                return View("MatchInfoWindow", match);
            }

            match.Participates = participateList;

            // Update match scores and mark finished
            match.Team1score = team1Score;
            match.Team2score = team2Score;
            match.Hashappened = true;

            var team1 = participateList[0].Team;
            var team2 = participateList[1].Team;

            match.FkTeamidTeam = team1Score > team2Score ? team1.IdTeam : team2.IdTeam;
            match.Title = $"{team1.Name} vs {team2.Name}";

            _matchService.UpdateMatch(match);

            var tour = _tournamentService.GetTournamentById(match.FkTournamentidTournament);

            Console.WriteLine($"teamnr = {tour.Teamnr}, match place in tournament = {match.Placeintournament}");
            if (match.Placeintournament == tour.Teamnr - 2) //nes placeInTournament skaciuojasi nuo 0, o ne nuo 1
            {
                _tournamentService.EndTournament(tour);
            }
            else
            {
                var participate1 = new Participate
                {
                    FkMatchidMatch = match.FkMatchidMatch.Value,
                    FkMatchfkTournamentidTournament = match.FkTournamentidTournament,
                    FkTeamidTeam = match.FkTeamidTeam.Value,
                };
                _participateService.IndicateWinnerTeam(participate1);
            }

            return RedirectToAction("TournamentInfoWindow", "Tournament", new { id = match.FkTournamentidTournament });

        }



    }


}