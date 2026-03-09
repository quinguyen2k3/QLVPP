using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Constants.Types;
using QLVPP.Data;
using QLVPP.DTOs.Request;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class StockOutRepository(AppDbContext context)
        : BaseRepository<StockOut>(context),
            IStockOutRepository
    {
        private readonly AppDbContext _context = context;

        private IQueryable<StockOut> BuildQuery(StockOutFilterReq condition)
        {
            var query = _context.StockOuts.AsNoTracking().AsQueryable();

            if (condition.Type.HasValue)
            {
                query = query.Where(x => x.Type == condition.Type);
            }

            if (condition.FromWarehouseId.HasValue)
            {
                query = query.Where(x => x.WarehouseId == condition.FromWarehouseId.Value);
            }

            if (condition.ToWarehouseId.HasValue)
            {
                query = query.Where(x => x.ToWarehouseId == condition.ToWarehouseId.Value);
            }

            if (condition.DepartmentId.HasValue)
            {
                query = query.Where(x => x.DepartmentId == condition.DepartmentId.Value);
            }

            if (condition.Statuses != null && condition.Statuses.Count > 0)
            {
                query = query.Where(x => condition.Statuses.Contains(x.Status));
            }

            if (!string.IsNullOrEmpty(condition.CreatedBy))
            {
                query = query.Where(x => x.CreatedBy == condition.CreatedBy);
            }

            if (condition.IsActivated.HasValue)
            {
                query = query.Where(x => x.IsActivated == condition.IsActivated.Value);
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

        public override async Task<StockOut?> GetById(object id)
        {
            var Id = Convert.ToInt64(id);

            return await _context
                .StockOuts.Include(r => r.StockOutDetails)
                .ThenInclude(d => d.Product)
                .ThenInclude(p => p.Unit)
                .FirstOrDefaultAsync(r => r.Id == Id);
        }

        public async Task<List<StockOut>> GetByConditions(StockOutFilterReq filter)
        {
            return await BuildQuery(filter).ToListAsync();
        }

        public async Task<bool> HasUnfinishedDocs(long warehouseId, DateOnly toDate)
        {
            return await _context.StockOuts.AnyAsync(x =>
                x.WarehouseId == warehouseId
                && x.StockOutDate <= toDate
                && x.Status != StockOutStatus.Received
                && x.Status != StockOutStatus.Cancelled
            );
        }

        public async Task<StockOut?> GetByCode(string code)
        {
            return await _context
                .StockOuts.Include(x => x.StockOutDetails)
                .FirstOrDefaultAsync(x => x.Code == code);
        }
    }
}
