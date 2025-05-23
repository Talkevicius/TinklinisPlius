using ErrorOr;
using TinklinisPlius.Models;
namespace TinklinisPlius.Services.Tournament
{
    public interface ITournamentService
    {
        ErrorOr<Created> CreateTournament(Models.Tournament tournament);
        ErrorOr<List<Models.Tournament>> GetAllTournaments();
        public Models.Tournament GetTournamentById(int id);
    }
}