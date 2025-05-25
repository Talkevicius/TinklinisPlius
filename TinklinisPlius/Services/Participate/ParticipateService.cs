using ErrorOr;
using TinklinisPlius.Models;
using Microsoft.EntityFrameworkCore;

namespace TinklinisPlius.Services.Participate
{
    public class ParticipateService : IParticipateService
    {
        private readonly AppDbContext _context;

        public ParticipateService(AppDbContext context)
        {
            _context = context;
        }

        public void CreateParticipate(List<Models.Participate> participates)
        {
            if (participates == null || !participates.Any())
                throw new ArgumentNullException(nameof(participates));

            _context.Participates.AddRange(participates);
            _context.SaveChanges();
        }


        public ErrorOr<List<Models.Participate>> GetParticipatesByMatchId(int matchId)
        {
            try
            {
                var participates = _context.Participates
                    .Where(p => p.FkMatchidMatch == matchId)
                    .Include(p => p.Team)                  // Include related Team
                    .ThenInclude(t => t.Players)       // Include Team's Players
                    .ToList();

                return participates;
            }
            catch (Exception ex)
            {
                return Error.Unexpected(description: $"Failed to fetch participates: {ex.Message}");
            }
        }

        public void IndicateWinnerTeam(Models.Participate participate)
        {
            if (participate == null)
                throw new ArgumentNullException(nameof(participate));

            _context.Participates.Add(participate);
            _context.SaveChanges();
        }

        
    }
}