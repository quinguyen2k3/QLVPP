using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.DTOs.Request;
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

        private IQueryable<StockIn> BuildQuery(StockInFilterReq condition)
        {
            var query = _context.StockIns.AsQueryable();

            if (condition.WarehouseId.HasValue)
            {
                query = query.Where(x => x.WarehouseId == condition.WarehouseId);
            }
            if (!string.IsNullOrEmpty(condition.CreatedBy))
            {
                query = query.Where(x => x.CreatedBy == condition.CreatedBy);
            }
            if (condition.IsActivated != null)
            {
                query = query.Where(x => x.IsActivated == condition.IsActivated);
            }
            if (condition.Statuses != null && condition.Statuses.Count > 0)
            {
                query = query.Where(x => condition.Statuses.Contains(x.Status));
            }

            if (condition.OrderByDesc)
            {
                query = query.OrderByDescending(x => x.CreatedDate);
            }
            else
            {
                query = query.OrderBy(x => x.CreatedDate);
            }

            return query;
        }

        public async Task<List<StockIn>> GetByConditions(StockInFilterReq filter)
        {
            return await BuildQuery(filter).ToListAsync();
        }

        public override async Task<StockIn?> GetById(object id)
        {
            var Id = Convert.ToInt64(id);

            return await _context
                .StockIns.Include(o => o.StockInDetails)
                .ThenInclude(d => d.Product)
                .ThenInclude(p => p.Unit)
                .FirstOrDefaultAsync(o => o.Id == Id);
        }

        public async Task<bool> HasUnapprovedDocs(long warehouseId, DateOnly toDate)
        {
            return await _context.StockIns.AnyAsync(x =>
                x.WarehouseId == warehouseId
                && x.StockInDate <= toDate
                && x.Status != StockInStatus.Approve
                && x.Status != StockInStatus.Cancelled
            );
        }

        public async Task<StockIn?> GetByCode(string code)
        {
            return await _context
                .StockIns.Include(o => o.StockInDetails)
                .FirstOrDefaultAsync(x => x.Code == code);
        }
    }
}
