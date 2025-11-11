using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class StockOutRepository(AppDbContext context)
        : BaseRepository<StockOut>(context),
            IStockOutRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<StockOut>> GetPendingByWarehouseId(long id)
        {
            return await _context
                .Deliveries.Where(d => d.WarehouseId == id && d.Status == StockOutStatus.Pending)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<StockOut>> GetByCreator(string creator)
        {
            return await _context
                .Deliveries.Where(d => d.CreatedBy == creator && d.IsActivated == true)
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();
        }

        public override async Task<StockOut?> GetById(object id)
        {
            var Id = Convert.ToInt64(id);

            return await _context
                .Deliveries.Include(r => r.StockOutDetails)
                .ThenInclude(d => d.Product)
                .ThenInclude(p => p.Unit)
                .FirstOrDefaultAsync(r => r.Id == Id);
        }

        public async Task<List<StockOut>> GetApprovedByDepartmentId(long id)
        {
            return await _context
                .Deliveries.Where(d => d.DepartmentId == id && d.Status == StockOutStatus.Approved)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<StockOut>> GetByWarehouseId(long id)
        {
            return await _context
                .Deliveries.Where(d => d.WarehouseId == id && d.IsActivated == true)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }
    }
}
