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
                .OrderBy(m => m.Placeintournament) 
                .ToList();
        }
        public void CreateMatch(Models.Match match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            _context.Matches.Add(match);
            _context.SaveChanges();
        }

        public int GetMatchesByPlaceInTournament(int tournamentId, int place)
        {
            var result= _context.Matches
                .Where(m => m.FkTournamentidTournament == tournamentId && m.Placeintournament==place);
            return result.ToList().First().IdMatch;
        }
        
        public void UpdateMatch(Models.Match updatedMatch)
        {
            var existingMatch = _context.Matches.FirstOrDefault(m => m.IdMatch == updatedMatch.IdMatch);

            if (existingMatch != null)
            {
                existingMatch.FkMatchidMatch = updatedMatch.FkMatchidMatch;
                existingMatch.FkMatchfkTournamentidTournament = updatedMatch.FkMatchfkTournamentidTournament;

                // Optional: update other fields if needed
                existingMatch.Title = updatedMatch.Title;
                existingMatch.Placeintournament = updatedMatch.Placeintournament;

                _context.SaveChanges();
            }
        }
        
        public Models.Match GetMatchById(int id)
        {
            return _context.Matches.FirstOrDefault(t => t.IdMatch == id);
        }




    }
}