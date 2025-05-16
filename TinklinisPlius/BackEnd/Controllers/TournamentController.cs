using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Models;

namespace TinklinisPlius.Controllers
{
    public class TournamentController : Controller
    {
        // Mock data for tournaments
        private List<Tournament> GetMockTournaments()
{
    return new List<Tournament>
    {
        new Tournament
        {
            IdTournament = 1,
            Title = "Spring Cup",
            Startdate = DateOnly.FromDateTime(DateTime.Today),
            Enddate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            Country = "Lithuania",
            Teamnr = 4,
            Creationtype = true,
            Isactive = true,
            Matches = new List<Match>
            {
                new Match
                {
                    IdMatch = 1,
                    Title = "Team A vs Team B",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                    Team1score = 3,
                    Team2score = 2,
                    Hashappened = true,
                    Placeintournament = 1
                },
                new Match
                {
                    IdMatch = 2,
                    Title = "Team C vs Team D",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                    Team1score = 1,
                    Team2score = 4,
                    Hashappened = true,
                    Placeintournament = 2
                },
                new Match
                {
                    IdMatch = 3,
                    Title = "Team A vs Team D",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                    Team1score = 2,
                    Team2score = 0,
                    Hashappened = true,
                    Placeintournament = 3
                }
            }
        },
        new Tournament
        {
            IdTournament = 2,
            Title = "Autumn Open",
            Startdate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
            Enddate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1).AddDays(3)),
            Country = "Latvia",
            Teamnr = 4,
            Creationtype = false,
            Isactive = false,
            Matches = new List<Match>
            {
                new Match
                {
                    IdMatch = 4,
                    Title = "Team E vs Team F",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(1).AddDays(1)),
                    Team1score = null,
                    Team2score = null,
                    Hashappened = false,
                    Placeintournament = 1
                },
                new Match
                {
                    IdMatch = 5,
                    Title = "Team G vs Team H",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(1).AddDays(2)),
                    Team1score = null,
                    Team2score = null,
                    Hashappened = false,
                    Placeintournament = 2
                },
                new Match
                {
                    IdMatch = 6,
                    Title = "Winner SF1 vs Winner SF2",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(1).AddDays(3)),
                    Team1score = null,
                    Team2score = null,
                    Hashappened = false,
                    Placeintournament = 3
                }
            }
        },
        // Tournament with 8 matches
        new Tournament
        {
            IdTournament = 3,
            Title = "Winter Challenge",
            Startdate = DateOnly.FromDateTime(DateTime.Today.AddMonths(2)),
            Enddate = DateOnly.FromDateTime(DateTime.Today.AddMonths(2).AddDays(5)),
            Country = "Estonia",
            Teamnr = 8,
            Creationtype = true,
            Isactive = true,
            Matches = new List<Match>
            {
                new Match
                {
                    IdMatch = 7,
                    Title = "Team A vs Team B",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(2).AddDays(1)),
                    Team1score = 3,
                    Team2score = 2,
                    Hashappened = true,
                    Placeintournament = 1
                },
                new Match
                {
                    IdMatch = 8,
                    Title = "Team C vs Team D",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(2).AddDays(2)),
                    Team1score = 1,
                    Team2score = 4,
                    Hashappened = true,
                    Placeintournament = 2
                },
                new Match
                {
                    IdMatch = 9,
                    Title = "Team E vs Team F",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(2).AddDays(3)),
                    Team1score = 0,
                    Team2score = 3,
                    Hashappened = true,
                    Placeintournament = 3
                },
                new Match
                {
                    IdMatch = 10,
                    Title = "Team G vs Team H",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(2).AddDays(4)),
                    Team1score = 2,
                    Team2score = 1,
                    Hashappened = true,
                    Placeintournament = 4
                },
                new Match
                {
                    IdMatch = 11,
                    Title = "Winner QF1 vs Winner QF2",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(2).AddDays(5)),
                    Team1score = null,
                    Team2score = null,
                    Hashappened = false,
                    Placeintournament = 5
                },
                new Match
                {
                    IdMatch = 12,
                    Title = "Winner QF3 vs Winner QF4",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(2).AddDays(6)),
                    Team1score = null,
                    Team2score = null,
                    Hashappened = false,
                    Placeintournament = 6
                },
                new Match()
                {
                IdMatch = 13,
                Title = "Winner QF1 vs Winner QF2",
                Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(2).AddDays(5)),
                Team1score = null,
                Team2score = null,
                Hashappened = false,
                Placeintournament = 7
            },
            }
        },
        // Tournament with 16 matches
        new Tournament
        {
            IdTournament = 4,
            Title = "Grand Masters",
            Startdate = DateOnly.FromDateTime(DateTime.Today.AddMonths(3)),
            Enddate = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(7)),
            Country = "Germany",
            Teamnr = 16,
            Creationtype = false,
            Isactive = true,
            Matches = new List<Match>
            {
        new Match { IdMatch = 13, Title = "Team A vs Team B", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(1)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 1 },
        new Match { IdMatch = 14, Title = "Team C vs Team D", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(2)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 2 },
        new Match { IdMatch = 15, Title = "Team E vs Team F", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(3)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 3 },
        new Match { IdMatch = 16, Title = "Team G vs Team H", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(4)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 4 },
        new Match { IdMatch = 17, Title = "Team I vs Team J", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(5)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 5 },
        new Match { IdMatch = 18, Title = "Team K vs Team L", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(6)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 6 },
        new Match { IdMatch = 19, Title = "Team M vs Team N", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(7)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 7 },
        new Match { IdMatch = 20, Title = "Team O vs Team P", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(8)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 8 },
        new Match { IdMatch = 21, Title = "Winner Match 1 vs Winner Match 2", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(9)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 9 },
        new Match { IdMatch = 22, Title = "Winner Match 3 vs Winner Match 4", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(10)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 10 },
        new Match { IdMatch = 23, Title = "Winner Match 5 vs Winner Match 6", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(11)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 11 },
        new Match { IdMatch = 24, Title = "Winner Match 7 vs Winner Match 8", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(12)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 12 },
        new Match { IdMatch = 25, Title = "Winner Match 9 vs Winner Match 10", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(13)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 13 },
        new Match { IdMatch = 26, Title = "Winner Match 11 vs Winner Match 12", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(14)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 14 },
        new Match { IdMatch = 27, Title = "Final Match: Winner Match 13 vs Winner Match 14", Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(3).AddDays(15)), Team1score = null, Team2score = null, Hashappened = false, Placeintournament = 15 }
    }
        }
    };
}


        // Action to show the tournament list
        public ActionResult TournamentListWindow()
        {
            var tournaments = GetMockTournaments(); // Replace with DB call later
            return View(tournaments);
        }

        // Action to show details of a selected tournament
        public ActionResult TournamentInfoWindow(int id)
        {
            var tournaments = GetMockTournaments(); // Mock data
            var tournament = tournaments.FirstOrDefault(t => t.IdTournament == id); // Find tournament by ID
            
            // Pass the tournament data along with its matches
            return View(tournament); // View with tournament and matches
        }
    }
}
