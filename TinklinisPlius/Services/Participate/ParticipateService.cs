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

        public void CreateParticipate(Models.Participate p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
    
            _context.Participates.Add(p);
            _context.SaveChanges();
    
            
        }



        
    }
}