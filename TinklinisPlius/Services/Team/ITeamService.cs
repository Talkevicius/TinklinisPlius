using ErrorOr;
using TinklinisPlius.Models;
namespace TinklinisPlius.Services.Team
{



    public interface ITeamService
    {
        ErrorOr<List<Models.Team>> GetAllTeams();
        ErrorOr<Created> CreateTeam(Models.Team team);
        ErrorOr<List<Models.Team>> GetAvailableTeams();
        List<Models.Team> GetTeamsByIds(int[] ids);

        // 🔽 PRIDĖTA:
        bool TeamExistsByName(string name);
    }


}