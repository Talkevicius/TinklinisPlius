using ErrorOr;
using TinklinisPlius.Models;
namespace TinklinisPlius.Services.Team
{



    public interface ITeamService
    {
        ErrorOr<List<Models.Team>> GetAllTeams();
        ErrorOr<Created> AddTeam(Models.Team team);
        ErrorOr<List<Models.Team>> GetAvailableTeams();
        List<Models.Team> GetTeamsByIds(int[] ids);
        void Save();

        ErrorOr<Updated> SetEloTo1(Models.Team team, int elo);
        // 🔽 PRIDĖTA:
        bool TeamExistsByName(string name);
        
       

    }


}