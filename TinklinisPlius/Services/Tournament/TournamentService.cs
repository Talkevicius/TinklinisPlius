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
        public ErrorOr<Created> CreateTournament(Models.Tournament tournament)
        {
            try
            {
                if (tournament == null)
                    return ErrorOr.Error.Failure();

                if (_context.Tournaments.Find(tournament.IdTournament) != null)
                    return ErrorOr.Error.Failure();


                _context.Tournaments.Add(tournament);
                _context.SaveChanges();
                return Result.Created;
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


        

        
    }
}
