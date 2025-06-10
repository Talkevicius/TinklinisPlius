using ErrorOr;
using TinklinisPlius.Models;
using Microsoft.EntityFrameworkCore;
using TinklinisPlius.Models;

namespace TinklinisPlius.Services.Team
{
    public class TeamService : ITeamService
    {
        private readonly AppDbContext _context;

        public TeamService(AppDbContext context)
        {
            _context = context;
        }

        public ErrorOr<List<Models.Team>> GetAllTeams()
        {
            List<Models.Team> teams = _context.Teams.ToList();
            return teams;
        }

        public ErrorOr<Created> AddTeam(Models.Team team)
        {
            try
            {
                _context.Teams.Add(team);
                _context.SaveChanges();

                return Result.Created;
            }
            catch (DbUpdateException ex)
            {
                // Optional: log exception here
                return Error.Unexpected(
                    code: "Team.CreationFailed",
                    description: "Failed to create team due to a database error.");
            }
            catch (Exception ex)
            {
                return Error.Unexpected(
                    code: "Team.UnknownError",
                    description: "An unknown error occurred while creating the team.");
            }
        }

        public ErrorOr<List<Models.Team>> GetAvailableTeams()
        {
            try
            {
                var teams = _context.Teams
                    .Where(t => t.Isparticipating == false)
                    .ToList();

                return teams;
            }
            catch (Exception ex)
            {
                // Optional: log exception
                return Error.Failure(
                    code: "Team.GetAvailableTeamsFailed",
                    description: "Could not retrieve available teams.");
            }
        }

        public List<Models.Team> GetTeamsByIds(int[] ids)
        {
            return _context.Teams.Where(t => ids.Contains(t.IdTeam)).ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool TeamExistsByName(string name)
        {
            return _context.Teams.Any(t => t.Name.ToLower() == name.Trim().ToLower());
        }

        public ErrorOr<Updated> SetEloTo1(Models.Team team, int elo)
        {
            team.Elo = 1;
            _context.SaveChanges();
            return Result.Updated;
        }
        
    }
}