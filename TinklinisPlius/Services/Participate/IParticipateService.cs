using ErrorOr;
using TinklinisPlius.Models;
namespace TinklinisPlius.Services.Participate
{
    public interface IParticipateService
    {
        public void CreateParticipate(List<Models.Participate> participates);
        public void IndicateWinnerTeam(Models.Participate participate);
        ErrorOr<List<Models.Participate>> GetParticipatesByMatchId(int matchId);
    }
}