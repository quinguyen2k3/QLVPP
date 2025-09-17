using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;
using System.Reflection.Metadata.Ecma335;

namespace QLVPP.Repositories.Implementations
{
    public class UnitRepository : BaseRepository<Unit>, IUnitRepository
    {
        private readonly AppDbContext _context;
        public UnitRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Unit>> GetAllIsActived()
        {
            return await _context.Units
                .Where(u => u.IsActived == true)
                .ToListAsync();
        }
    }
}
