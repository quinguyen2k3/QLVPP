using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class StockInRepository : BaseRepository<StockIn>, IStockInRepository
    {
        private readonly AppDbContext _context;

        public StockInRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<StockIn>> GetByCreator(string creator)
        {
            return await _context
                .StockIns.Where(o => o.CreatedBy == creator && o.IsActivated == true)
                .OrderByDescending(o => o.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<StockIn>> GetPendingByWarehouseId(long id)
        {
            return await _context
                .StockIns.Where(o => o.WarehouseId == id && o.Status == StockInStatus.Pending)
                .OrderByDescending(o => o.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<StockIn>> GetByWarehouseId(long id)
        {
            return await _context
                .StockIns.Where(o => o.WarehouseId == id && o.IsActivated == true)
                .OrderByDescending(o => o.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<StockIn?> GetById(object id)
        {
            var Id = Convert.ToInt64(id);

            return await _context
                .StockIns.Include(o => o.StockInDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == Id);
        }
    }
}
