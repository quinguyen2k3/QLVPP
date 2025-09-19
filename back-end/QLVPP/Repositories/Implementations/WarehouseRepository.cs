using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
    {
        private readonly AppDbContext _context;
        public WarehouseRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Warehouse>> GetAllIsActived()
        {
             return await _context.Warehouses.Where(w => w.IsActived == true)
                .OrderByDescending(w => w.CreatedDate)
                .ToListAsync();
        }
    }
}
