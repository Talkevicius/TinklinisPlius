using Microsoft.AspNetCore.Mvc;
using TinklinisPlius.Services.Player;

namespace TinklinisPlius.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        public IActionResult PlayerElo()
        {
            var result = _playerService.GetAllPlayers();
            if (result.IsError)
            {
                return View("Error");
            }

            return View(result.Value);
        }
    }
}
