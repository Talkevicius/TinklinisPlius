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
                ModelState.AddModelError("", "A draw is not allowed. Please enter a winning score.");
                match.Participates = participateList;
                return View("MatchInfoWindow", match);
            }

            match.Participates = participateList;
            match.Team1score = team1Score;
            match.Team2score = team2Score;
            match.Hashappened = true;

            var team1 = participateList[0].Team;
            var team2 = participateList[1].Team;

            int elo1 = team1.Elo ?? 0;
            int elo2 = team2.Elo ?? 0;

            bool team1Won = team1Score > team2Score;
            bool team2Won = !team1Won;

            double updatedElo1 = elo1;
            double updatedElo2 = elo2;

            if (elo1 == elo2)
            {
                // Kai ELO vienodi
                if (team1Won)
                {
                    updatedElo1 += 1 - ((double)team2Score / team1Score) * 100 + 10;
                    updatedElo2 += 1 - ((double)team1Score / team2Score) * 100 + 10;
                }
                else
                {
                    updatedElo1 += 1 - ((double)team2Score / team1Score) * 100 + 10;
                    updatedElo2 += 1 - ((double)team1Score / team2Score) * 100 + 10;
                }
            }
            else
            {
                // Kai ELO skirtingi
                double chanceTeam1 = (1.0 / (1 + Math.Pow(10, (elo2 - elo1) / 400.0))) * 100;
                double chanceTeam2 = (1.0 / (1 + Math.Pow(10, (elo1 - elo2) / 400.0))) * 100;

                if (team1Won)
                {
                    updatedElo1 += 1 - chanceTeam1;
                    updatedElo2 -= chanceTeam2;
                }
                else
                {
                    updatedElo1 -= chanceTeam1;
                    updatedElo2 += 1 - chanceTeam2;
                }
            }

            // Nustatome nugal?toj?
            match.FkTeamidTeam = team1Won ? team1.IdTeam : team2.IdTeam;

            // Atnaujiname ELO
            team1.Elo = (int)Math.Round(updatedElo1);
            team2.Elo = (int)Math.Round(updatedElo2);

            _context.Update(team1);
            _context.Update(team2);
            _matchService.UpdateMatch(match);
            _context.SaveChanges();

            var tour = _tournamentService.GetTournamentById(match.FkTournamentidTournament);

            if (match.Placeintournament == tour.Teamnr - 1)
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