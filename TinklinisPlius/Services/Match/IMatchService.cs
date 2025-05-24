using ErrorOr;
using TinklinisPlius.Models;
namespace TinklinisPlius.Services.Match
{
    public interface IMatchService
    {
        public ICollection<Models.Match> GetMatchesByTournamentId(int tournamentId);
        public void CreateMatch(Models.Match match);
        public int GetMatchesByPlaceInTournament(int tournamentId,int place);
        public void UpdateMatch(Models.Match updatedMatch);
        public Models.Match GetMatchById(int id);
    }
}