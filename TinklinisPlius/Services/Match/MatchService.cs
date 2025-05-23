using ErrorOr;
using TinklinisPlius.Models;
using Microsoft.EntityFrameworkCore;
using TinklinisPlius.Models;

namespace TinklinisPlius.Services.Match
{
    public class MatchService : IMatchService
    {
        private readonly AppDbContext _context;

        public MatchService(AppDbContext context)
        {
            _context = context;
        }
        
        public ICollection<Models.Match> GetMatchesByTournamentId(int tournamentId)
        {
            return _context.Matches
                .Where(m => m.FkTournamentidTournament == tournamentId)
                .ToList();
        }


        
    }
}