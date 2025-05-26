using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Models;
using Npgsql;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;

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
    [ActionName("CreateWagerWindow")]
    public IActionResult SubmitWagerCreation(WagerCreationViewModel model)
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
            int team1Id = 0;
            int team2Id = 0;
            int team1Elo = 0;
            int team2Elo = 0;

            using (var cmd = new NpgsqlCommand("SELECT fk_Tournamentid_Tournament FROM Match WHERE id_Match = @matchId", conn))
            {
                cmd.Parameters.AddWithValue("@matchId", model.SelectedMatchId);
                tournamentId = (int)cmd.ExecuteScalar();
            }

            using (var cmd = new NpgsqlCommand(@"
                SELECT fk_Teamid_Team 
                FROM participates 
                WHERE fk_Matchid_Match = @matchId AND fk_Matchfk_Tournamentid_Tournament = @tournamentId", conn))
            {
                cmd.Parameters.AddWithValue("@matchId", model.SelectedMatchId);
                cmd.Parameters.AddWithValue("@tournamentId", tournamentId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        team1Id = reader.GetInt32(0);
                    if (reader.Read())
                        team2Id = reader.GetInt32(0);
                }
            }

            using (var cmd = new NpgsqlCommand(@"
                SELECT id_Team, elo 
                FROM Team 
                WHERE id_Team = @team1Id OR id_Team = @team2Id", conn))
            {
                cmd.Parameters.AddWithValue("@team1Id", team1Id);
                cmd.Parameters.AddWithValue("@team2Id", team2Id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        int elo = reader.IsDBNull(1) ? 1000 : reader.GetInt32(1);

                        if (id == team1Id)
                            team1Elo = elo;
                        else if (id == team2Id)
                            team2Elo = elo;
                    }
                }
            }

            double expectedScore = 1.0 / (1.0 + Math.Pow(10, (team2Elo - team1Elo) / 400.0));
            int chancePercent = (int)Math.Round(expectedScore * 100);
            using (var cmd = new NpgsqlCommand(@"
                INSERT INTO Wager (chance, combinedSum, hasFinished, fk_Matchid_Match, fk_Matchfk_Tournamentid_Tournament)
                VALUES (@chance, 0, false, @matchId, @tournamentId)", conn))
            {
                cmd.Parameters.AddWithValue("@chance", chancePercent);
                cmd.Parameters.AddWithValue("@matchId", model.SelectedMatchId);
                cmd.Parameters.AddWithValue("@tournamentId", tournamentId);
                cmd.ExecuteNonQuery();
            }
        }

        TempData["SuccessMessage"] = "Wager created successfully.";
        return RedirectToAction("CreateWagerWindow");
    }

    public IActionResult CheckRiskBeforeBet(int wagerId)
    {
        int userId = 1;

        float lossRate = 0f, betVariance = 0f, highOddsFrequency = 0f, riskFactor = 0f;

        using (var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            conn.Open();

            var results = new List<bool>();
            var betAmounts = new List<int>();
            var odds = new List<int>();

            using (var cmd = new NpgsqlCommand(@"
                SELECT result, moneyBetted, chanceOfWinning 
                FROM Bet 
                WHERE fk_Userid_User = @userId", conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0)) results.Add(reader.GetBoolean(0));
                        if (!reader.IsDBNull(1)) betAmounts.Add(reader.GetInt32(1));
                        if (!reader.IsDBNull(2)) odds.Add(reader.GetInt32(2));
                    }
                }
            }

            int totalResolved = results.Count;
            int losses = results.Count(r => r == false);
            lossRate = totalResolved > 0 ? (float)losses / totalResolved * 100f : 0f;

            if (betAmounts.Count > 0)
            {
                float mean = (float)betAmounts.Average();
                float variance = betAmounts.Select(a => (a - mean) * (a - mean)).Sum() / betAmounts.Count;
                betVariance = variance % 10;
            }

            if (odds.Count > 0)
            {
                int highRiskBets = odds.Count(o => o < 30 || o > 70);
                highOddsFrequency = (float)highRiskBets / odds.Count * 100f;
            }

            // Risk factor
            riskFactor = (lossRate * 0.5f) + (betVariance * 0.3f) + (highOddsFrequency * 0.2f);

            using (var updateCmd = new NpgsqlCommand(@"
                UPDATE ""User""
                SET riskfactor = @riskFactor,
                    lossRate = @lossRate,
                    betVariance = @betVariance,
                    highOddsFrequency = @highOddsFrequency
                WHERE id_User = @userId", conn))
            {
                updateCmd.Parameters.AddWithValue("@riskFactor", riskFactor);
                updateCmd.Parameters.AddWithValue("@lossRate", lossRate);
                updateCmd.Parameters.AddWithValue("@betVariance", betVariance);
                updateCmd.Parameters.AddWithValue("@highOddsFrequency", highOddsFrequency);
                updateCmd.Parameters.AddWithValue("@userId", userId);
                updateCmd.ExecuteNonQuery();
            }
        }

        if (riskFactor > 60)
        {
            TempData["ErrorMessage"] = "Your risk factor is too high. You are not allowed to place bets.";
            return RedirectToAction("WagerInfoWindow");
        }
        else if (riskFactor > 30)
        {
            TempData["WarningMessage"] = "Your risk factor is elevated. Please bet cautiously.";
        }

        return RedirectToAction("PlaceBetWindow", new { wagerId });
    }

    [HttpGet]
    public IActionResult PlaceBetWindow(int wagerId)
    {
        var model = new PlaceBetViewModel
        {
            WagerId = wagerId,
            UserId = 1,
            Teams = new List<SelectListItem>()
        };

        using (var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand(@"
                SELECT t.id_Team, t.name
                FROM Wager w
                JOIN Match m ON w.fk_Matchid_Match = m.id_Match AND w.fk_Matchfk_Tournamentid_Tournament = m.fk_Tournamentid_Tournament
                JOIN participates p ON p.fk_Matchid_Match = m.id_Match AND p.fk_Matchfk_Tournamentid_Tournament = m.fk_Tournamentid_Tournament
                JOIN Team t ON t.id_Team = p.fk_Teamid_Team
                WHERE w.id_Wager = @wagerId", conn))
            {
                cmd.Parameters.AddWithValue("@wagerId", wagerId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Teams.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
        }

        return View(model);
    }


    /*
    dotnet add package Stripe.net

    Card Number (works): 4242 4242 4242 4242
    (insufficient funds): 4000 0000 0000 9995
    */

    [HttpPost]
    public IActionResult PlaceBetWindow(PlaceBetViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

        var options = new ChargeCreateOptions
        {
            Amount = model.Amount * 100,
            Currency = "usd",
            Description = $"Bet on Wager #{model.WagerId}",
            Source = model.StripeToken
        };

        var service = new ChargeService();
        try
        {
            Charge charge = service.Create(options);

            if (charge.Status == "succeeded")
            {
                using (var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    int chance = 0;
                    using (var cmd = new NpgsqlCommand("SELECT chance FROM Wager WHERE id_Wager = @wagerId", conn))
                    {
                        cmd.Parameters.AddWithValue("@wagerId", model.WagerId);
                        chance = (int)cmd.ExecuteScalar();
                    }

                    using (var cmd = new NpgsqlCommand(@"
                        INSERT INTO Bet (moneyBetted, chanceOfWinning, payout, betDate, betType, result, fk_Wagerid_Wager, fk_Userid_User)
                        VALUES (@money, @chance, 0, @date, true, @result, @wagerId, @userId)", conn))
                    {
                        cmd.Parameters.AddWithValue("@money", model.Amount);
                        cmd.Parameters.AddWithValue("@chance", chance);
                        cmd.Parameters.AddWithValue("@date", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@result", DBNull.Value);
                        cmd.Parameters.AddWithValue("@wagerId", model.WagerId);
                        cmd.Parameters.AddWithValue("@userId", model.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Bet placed successfully.";
                return RedirectToAction("WagerInfoWindow");
            }
            else
            {
                ModelState.AddModelError("", "Payment failed. Try again.");
                return View(model);
            }
        }
        catch (StripeException ex)
        {
            string errorMessage = ex.StripeError?.Message ?? "Payment failed. Please try again.";

            ModelState.AddModelError("", $"Stripe error: {errorMessage}");
            return View(model);
        }
    }
}