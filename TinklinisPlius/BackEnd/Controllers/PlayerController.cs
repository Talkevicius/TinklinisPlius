using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinklinisPlius.Models;

namespace TinklinisPlius.Controllers
{
    public class PlayerController : Controller
    {
        private readonly AppDbContext _context;

        public PlayerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult PlayerDetails(int id)
        {
            var player = _context.Players
                .Include(p => p.FkTeamidTeamNavigation) // jeigu nori rodyti komandos duomenis
                .FirstOrDefault(p => p.IdPlayer == id);

            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }
    }
}
