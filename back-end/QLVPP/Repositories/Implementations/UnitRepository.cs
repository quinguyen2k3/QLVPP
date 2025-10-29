using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class UnitRepository : BaseRepository<Unit>, IUnitRepository
    {
        private readonly AppDbContext _context;

        public UnitRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Unit>> GetAllIsActivated()
        {
            return await _context
                .Units.Where(u => u.IsActivated == true)
                .OrderByDescending(u => u.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
