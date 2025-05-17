using ErrorOr;
using TinklinisPlius.Models;
using Microsoft.EntityFrameworkCore;
using TinklinisPlius.Models;

namespace TinklinisPlius.Services.Match
{
    public class MatchService : IMatchService
    {
        private readonly AppDbContext _context;

        public MatchService(AppDbContext context)
        {
            _context = context;
        }
        
    }
}