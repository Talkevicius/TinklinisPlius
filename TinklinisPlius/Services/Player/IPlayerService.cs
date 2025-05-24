using ErrorOr;
using System.Collections.Generic;
using TinklinisPlius.Models;

namespace TinklinisPlius.Services.Player
{
    public interface IPlayerService
    {
        ErrorOr<List<Models.Player>> GetAllPlayers();
        // Other methods if any...
    }
}
