using ErrorOr;
using TinklinisPlius.Models;
namespace TinklinisPlius.Services.Match
{
    public interface IMatchService
    {
        public ICollection<Models.Match> GetMatchesByTournamentId(int tournamentId);

    }
}