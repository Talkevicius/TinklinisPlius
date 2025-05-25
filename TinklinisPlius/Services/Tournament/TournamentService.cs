using ErrorOr;
using TinklinisPlius.Models;
using Microsoft.EntityFrameworkCore;
using TinklinisPlius.Models;

namespace TinklinisPlius.Services.Tournament
{
    public class TournamentService : ITournamentService
    {
        private readonly AppDbContext _context;

        public TournamentService(AppDbContext context)
        {
            _context = context;
        }
        public ErrorOr<int> CreateTournament(Models.Tournament tournament)
        {
            try
            {
                if (tournament == null)
                    return ErrorOr.Error.Failure();

                if (_context.Tournaments.Find(tournament.IdTournament) != null)
                    return ErrorOr.Error.Failure();


                _context.Tournaments.Add(tournament);
                _context.SaveChanges();
                return tournament.IdTournament;
            }
            catch
            {
                return ErrorOr.Error.Failure();
            }
        }

        public ErrorOr<List<Models.Tournament>> GetAllTournaments()
        {
            List<Models.Tournament> ret = _context.Tournaments.ToList();
    
            return ret; // Implicitly wrapped as successful ErrorOr<T>
        }
        
        // Assuming this is in your TournamentService class
        public Models.Tournament GetTournamentById(int id)
        {
            return _context.Tournaments.FirstOrDefault(t => t.IdTournament == id);
        }

        public void EndTournament(Models.Tournament tour)
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour));

            tour.Isactive = false;
            _context.Tournaments.Update(tour);  // Optional: only needed if not tracked
            _context.SaveChanges();
        }



    }
}