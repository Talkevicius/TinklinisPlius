using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Models;
using Npgsql;
using Microsoft.AspNetCore.Mvc.Rendering;

public class BettingController : Controller
{
    private readonly IConfiguration _configuration;

    public BettingController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// Wagers (Admin)
    public IActionResult WagerListWindow()
    {
        var wagers = new List<Wager>();
        string connString = _configuration.GetConnectionString("DefaultConnection");

        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand("SELECT * FROM Wager", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    wagers.Add(new Wager
                    {
                        IdWager = reader.GetInt32(reader.GetOrdinal("id_Wager")),
                        Chance = reader.IsDBNull(reader.GetOrdinal("chance")) ? null : reader.GetInt32(reader.GetOrdinal("chance")),
                        Combinedsum = reader.IsDBNull(reader.GetOrdinal("combinedSum")) ? null : reader.GetInt32(reader.GetOrdinal("combinedSum")),
                        Hasfinished = reader.IsDBNull(reader.GetOrdinal("hasFinished")) ? null : reader.GetBoolean(reader.GetOrdinal("hasFinished")),
                        FkMatchidMatch = reader.GetInt32(reader.GetOrdinal("fk_Matchid_Match")),
                        FkMatchfkTournamentidTournament = reader.GetInt32(reader.GetOrdinal("fk_Matchfk_Tournamentid_Tournament"))
                    });
                }
            }
        }

        return View(wagers);
    }

    /// Wager history (Admin)
    public IActionResult WagerHistoryWindow()
    {
    var finishedWagers = new List<Wager>();
    string connString = _configuration.GetConnectionString("DefaultConnection");

    using (var conn = new NpgsqlConnection(connString))
    {
        conn.Open();

        using (var cmd = new NpgsqlCommand("SELECT * FROM Wager WHERE hasFinished = TRUE", conn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                finishedWagers.Add(new Wager
                {
                    IdWager = reader.GetInt32(reader.GetOrdinal("id_Wager")),
                    Chance = reader.IsDBNull(reader.GetOrdinal("chance")) ? null : reader.GetInt32(reader.GetOrdinal("chance")),
                    Combinedsum = reader.IsDBNull(reader.GetOrdinal("combinedSum")) ? null : reader.GetInt32(reader.GetOrdinal("combinedSum")),
                    Hasfinished = reader.IsDBNull(reader.GetOrdinal("hasFinished")) ? null : reader.GetBoolean(reader.GetOrdinal("hasFinished")),
                    FkMatchidMatch = reader.GetInt32(reader.GetOrdinal("fk_Matchid_Match")),
                    FkMatchfkTournamentidTournament = reader.GetInt32(reader.GetOrdinal("fk_Matchfk_Tournamentid_Tournament"))
                });
            }
        }
    }

    return View("WagerHistoryWindow", finishedWagers);
}

    /// Ongoing wagers (User)
    public IActionResult WagerInfoWindow()
{
    var ongoingWagers = new List<Wager>();
    string connString = _configuration.GetConnectionString("DefaultConnection");

    using (var conn = new NpgsqlConnection(connString))
    {
        conn.Open();

        using (var cmd = new NpgsqlCommand(@"
            SELECT w.* 
            FROM Wager w
            JOIN Match m ON w.fk_Matchid_Match = m.id_Match
            WHERE 
                w.hasFinished = FALSE
                AND m.date > CURRENT_DATE", conn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                ongoingWagers.Add(new Wager
                {
                    IdWager = reader.GetInt32(reader.GetOrdinal("id_Wager")),
                    Chance = reader.IsDBNull(reader.GetOrdinal("chance")) ? null : reader.GetInt32(reader.GetOrdinal("chance")),
                    Combinedsum = reader.IsDBNull(reader.GetOrdinal("combinedSum")) ? null : reader.GetInt32(reader.GetOrdinal("combinedSum")),
                    Hasfinished = reader.IsDBNull(reader.GetOrdinal("hasFinished")) ? null : reader.GetBoolean(reader.GetOrdinal("hasFinished")),
                    FkMatchidMatch = reader.GetInt32(reader.GetOrdinal("fk_Matchid_Match")),
                    FkMatchfkTournamentidTournament = reader.GetInt32(reader.GetOrdinal("fk_Matchfk_Tournamentid_Tournament"))
                });
            }
        }
    }

    return View("WagerInfoWindow", ongoingWagers);
}
    [HttpGet]
    public IActionResult CreateWagerWindow()
    {
        var model = new WagerCreationViewModel();
        string connString = _configuration.GetConnectionString("DefaultConnection");

        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            string sql = @"
                SELECT m.id_Match, m.fk_Tournamentid_Tournament, m.date 
                FROM Match m
                WHERE m.date > CURRENT_DATE 
                AND NOT EXISTS (
                    SELECT 1 FROM Wager w WHERE w.fk_Matchid_Match = m.id_Match
                )";

            using (var cmd = new NpgsqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var matchId = reader.GetInt32(0);
                    var date = reader.GetDateTime(2).ToString("yyyy-MM-dd");

                    model.MatchOptions.Add(new SelectListItem
                    {
                        Value = matchId.ToString(),
                        Text = $"Match {matchId} on {date}"
                    });
                }
            }
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult CreateWagerWindow(WagerCreationViewModel model)
    {
        if (model.SelectedMatchId == 0)
        {
            ModelState.AddModelError("", "Please select a match.");
            return View(model);
        }

        string connString = _configuration.GetConnectionString("DefaultConnection");

        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();

            int tournamentId = 0;

            // Get the tournament ID for the selected match
            using (var cmd = new NpgsqlCommand("SELECT fk_Tournamentid_Tournament FROM Match WHERE id_Match = @matchId", conn))
            {
                cmd.Parameters.AddWithValue("@matchId", model.SelectedMatchId);
                tournamentId = (int)cmd.ExecuteScalar();
            }

            // Insert the new wager
            using (var cmd = new NpgsqlCommand(@"INSERT INTO Wager (chance, combinedSum, hasFinished, fk_Matchid_Match, fk_Matchfk_Tournamentid_Tournament) 
                                                VALUES (0, 0, false, @matchId, @tournamentId)", conn))
            {
                cmd.Parameters.AddWithValue("@matchId", model.SelectedMatchId);
                cmd.Parameters.AddWithValue("@tournamentId", tournamentId);
                cmd.ExecuteNonQuery();
            }
        }

        return RedirectToAction("WagerListWindow");
    }
}