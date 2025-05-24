using ErrorOr;
using TinklinisPlius.Models;
namespace TinklinisPlius.Services.Team
{
    public interface ITeamService
    {
        public ErrorOr<List<Models.Team>> GetAllTeams();
        ErrorOr<Created> CreateTeam(Models.Team team);
        public ErrorOr<List<Models.Team>> GetAvailableTeams();
        public List<Models.Team> GetTeamsByIds(int[] ids);

    }
}