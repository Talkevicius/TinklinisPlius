using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Models;
using Npgsql;

public class BettingController : Controller
{
    private readonly IConfiguration _configuration;

    public BettingController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

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
}