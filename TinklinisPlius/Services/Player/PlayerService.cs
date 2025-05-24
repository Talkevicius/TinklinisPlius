using ErrorOr;
using Microsoft.EntityFrameworkCore;
using TinklinisPlius.Models;

namespace TinklinisPlius.Services.Player
{
    public class PlayerService : IPlayerService
    {
        private readonly AppDbContext _context;

        public PlayerService(AppDbContext context)
        {
            _context = context;
        }

        public ErrorOr<List<Models.Player>> GetAllPlayers()
        {
            try
            {
                var players = _context.Players.ToList();
                return players;
            }
            catch
            {
                return Error.Failure(
                    code: "Player.FetchFailed",
                    description: "Could not retrieve players.");
            }
        }
    }
}
