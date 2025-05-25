using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Models;
using TinklinisPlius.Services.Tournament;
using TinklinisPlius.Services.Match;
using TinklinisPlius.Services.Participate;
using TinklinisPlius.Services.Team;

namespace TinklinisPlius.Controllers
{
    public class TournamentController : Controller
    {
        private readonly ITournamentService _tournamentService;
        private readonly IMatchService _matchService;
        private readonly ITeamService _teamService;
        private readonly IParticipateService _participateService;
        private readonly AppDbContext _context; 

        public TournamentController(ITournamentService tournamentService, IMatchService matchService,ITeamService teamService,IParticipateService participateService, AppDbContext context)
        {
            _tournamentService = tournamentService;
            _matchService = matchService;
            _teamService = teamService;
            _participateService = participateService;
            _context = context;
        }

        // Action to show the tournament list
        public ActionResult TournamentListWindow()
        {
            
            var tournaments = _tournamentService.GetAllTournaments();
            if (tournaments.IsError)
            {
                
                return View("Error");
            }

            
            return View(tournaments.Value);

        }

        // Action to show details of a selected tournament
        public ActionResult TournamentInfoWindow(int id)
        {
            var tournament = _tournamentService.GetTournamentById(id);

            if (tournament == null)
            {
                return View("Error");
            }

            // Fetch related matches for the tournament (if needed)
            tournament.Matches = _matchService.GetMatchesByTournamentId(id); 
            return View(tournament);
        }


        
        
        [HttpGet]
        public ActionResult CreateTournamentWindow()
        {
            var teams = _teamService.GetAvailableTeams();
            ViewBag.AvailableTeams = teams.Value;
            return View(new Tournament());
        }

        
        [HttpPost]
        public ActionResult CreateTournamentWindow(Tournament tournament, int[] SelectedTeamIds)
        {
            if (SelectedTeamIds.Length != tournament.Teamnr)
            {
                ModelState.AddModelError("", $"You must select exactly {tournament.Teamnr} teams.");
                var teams = _teamService.GetAvailableTeams();
                ViewBag.AvailableTeams = teams.Value;
                return View(tournament);
            }
            
            if (!ModelState.IsValid)
            {
                // Reload teams for redisplay
                var teams = _teamService.GetAvailableTeams();
                ViewBag.AvailableTeams = teams.Value;
                return View(tournament);
            }
            
            // Load teams by selected IDs
            var selectedTeams = _teamService.GetTeamsByIds(SelectedTeamIds);
    
            // Set tournament navigation for each team and mark participating
            foreach (var team in selectedTeams)
            {
                team.FkTournamentidTournament = tournament.IdTournament;
                team.Isparticipating = true;
            }

            tournament.Teams = selectedTeams;
            
            return CreateTournament(tournament);
        }

        [NonAction]
        public ActionResult CreateTournament(Tournament tournament)
        {
            tournament.Isactive = true;
            List<Team> teamsToPlay = tournament.Teams.ToList();

            // Sort teams by coefficient 
            if (tournament.Creationtype == true)
            {
                var teamsWithCoeff = teamsToPlay.Select(team => new 
                {
                    Team = team,
                    Coefficient = CalculateCoefficientForTeam(team,tournament.Country) //// Calculate coefficients in controller
                }).ToList();
                teamsWithCoeff = SortTeamsByCoefficient(teamsWithCoeff, tc => tc.Coefficient);

                // Update tournament.Teams order
                tournament.Teams = teamsWithCoeff.Select(tc => tc.Team).ToList();
            }
            else
            {
                // Randomize the teamsToPlay list directly
                teamsToPlay = SortTeamsByRandom(teamsToPlay);
                tournament.Teams = teamsToPlay;
            }
             

            // Update the tournament teams order based on sorting
            tournament.Teams = teamsToPlay;

            teamsToPlay = tournament.Teams.ToList();
            var createdTournament = _tournamentService.CreateTournament(tournament);
            int newTournamentId = createdTournament.Value;
            int count = 0;
            int tournamentPlace = 0;
            while (teamsToPlay.Count >= 2)
            {
                Team team1 = teamsToPlay[0];
                Team team2 = teamsToPlay[1];

                // Create match with these two teams and assign tournament
                var match = new Match
                {
                    Title = $"{team1.Name} vs {team2.Name}",
                    FkTournamentidTournament = newTournamentId,
                    Hashappened = false,
                    Date = _tournamentService.GetTournamentById(newTournamentId).Startdate,
                    Team1score = null,
                    Team2score = null,
                    Placeintournament = tournamentPlace,
                    
                };
                
                    _matchService.CreateMatch(match);

                var participate1 = new Participate
                {
                    FkMatchidMatch = match.IdMatch,
                    FkMatchfkTournamentidTournament = match.FkTournamentidTournament,
                    FkTeamidTeam = team1.IdTeam
                };

                var participate2 = new Participate
                {
                    FkMatchidMatch = match.IdMatch,
                    FkMatchfkTournamentidTournament = match.FkTournamentidTournament,
                    FkTeamidTeam = team2.IdTeam
                };

                _participateService.CreateParticipate(new List<Participate> { participate1, participate2 });
                // Remove the two teams from the list
                teamsToPlay.RemoveAt(0);
                teamsToPlay.RemoveAt(0);
                count++;
                tournamentPlace++;
            }

            List<Match> allMatches = _matchService.GetMatchesByTournamentId(newTournamentId).ToList();
            int totalMatches = tournament.Teamnr.Value - 1;
            // Start creating template matches from index = count
            for (int i = count; i < totalMatches; i++)
            {
                int p = i;

                var match = new Match
                {
                    Title = $"Template Match P{p}",
                    FkTournamentidTournament = newTournamentId,
                    Hashappened = false,
                    Date = _tournamentService.GetTournamentById(newTournamentId).Startdate,
                    Team1score = null,
                    Team2score = null,
                    Placeintournament = p
                };

                _matchService.CreateMatch(match); // Sets match.IdMatch
            }
            
            for (int p = count; p < totalMatches; p++)
            {
                Match parentMatch = allMatches[p];

                // Calculate left and right child indices
                int leftChildIndex = 2 * p - (totalMatches + 1);
                int rightChildIndex = 2 * p - totalMatches;

                // Assign parent's ID to left child match
                if (leftChildIndex >= 0 && leftChildIndex < allMatches.Count)
                {
                    allMatches[leftChildIndex].FkMatchidMatch = parentMatch.IdMatch;
                    _matchService.UpdateMatch(allMatches[leftChildIndex]);
                }

                // Assign parent's ID to right child match
                if (rightChildIndex >= 0 && rightChildIndex < allMatches.Count)
                {
                    allMatches[rightChildIndex].FkMatchidMatch = parentMatch.IdMatch;
                    _matchService.UpdateMatch(allMatches[rightChildIndex]);
                }
            }


            return View("TournamentInfoWindow", tournament);
        }

        public int CalculateCoefficientForTeam(Team team, string country)
        {
            int c = 0;
            c += team.Elo.Value;
            if (team.Country == country)
            {
                c += 1;
            }
            return c;
        }

        public List<T> SortTeamsByCoefficient<T>(List<T> teamsWithCoeff, Func<T, int> getCoeff)
        {
            return teamsWithCoeff.OrderByDescending(getCoeff).ToList();
        }

        
        public List<Team> SortTeamsByRandom(List<Team> teams)
        {
            Random rnd = new Random();
            return teams.OrderBy(t => rnd.Next()).ToList();
        }


    }
}
