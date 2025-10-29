using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> GetAllIsActivated()
        {
            return await _context
                .Suppliers.Where(s => s.IsActivated == true)
                .OrderByDescending(s => s.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
