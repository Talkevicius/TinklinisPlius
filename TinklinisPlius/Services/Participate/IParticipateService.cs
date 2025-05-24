using ErrorOr;
using TinklinisPlius.Models;
namespace TinklinisPlius.Services.Participate
{
    public interface IParticipateService
    {
        public void CreateParticipate(Models.Participate participate);
    }
}