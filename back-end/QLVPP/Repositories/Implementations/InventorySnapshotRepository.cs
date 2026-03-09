using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.DTOs.Request;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class InventorySnapshotRepository
        : BaseRepository<InventorySnapshot>,
            IInventorySnapshotRepository
    {
        private readonly AppDbContext _context;

        public InventorySnapshotRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        private IQueryable<InventorySnapshot> BuildQuery(InventorySnapshotFilterReq condition)
        {
            var query = _context.InventorySnapshots.AsQueryable();

            if (condition.WarehouseId.HasValue)
            {
                query = query.Where(x => x.WarehouseId == condition.WarehouseId);
            }

            if (condition.FromDate.HasValue)
            {
                query = query.Where(x => x.ToDate >= condition.FromDate);
            }

            if (condition.ToDate.HasValue)
            {
                query = query.Where(x => x.ToDate <= condition.ToDate);
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

        public async Task<List<InventorySnapshot>> GetListByConditionAsync(
            InventorySnapshotFilterReq condition
        )
        {
            return await BuildQuery(condition).ToListAsync();
        }

        public async Task<InventorySnapshot?> GetOneByConditionAsync(
            InventorySnapshotFilterReq condition
        )
        {
            return await BuildQuery(condition).FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByConditionAsync(InventorySnapshotFilterReq condition)
        {
            return await BuildQuery(condition).AnyAsync();
        }

        public async Task<DateOnly?> GetLatestToDateAsync(long? warehouseId = null)
        {
            var query = _context.InventorySnapshots.AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(x => x.WarehouseId == warehouseId);
            }
            return await query.MaxAsync(x => (DateOnly?)x.ToDate);
        }

        public override async Task<InventorySnapshot?> GetById(object id)
        {
            long longId = Convert.ToInt64(id);
            return await _context
                .InventorySnapshots.Include(x => x.Warehouse)
                .Include(x => x.SnapshotDetails)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Unit)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == longId);
        }

        public async Task<long> CreateSnapshotProcedure(
            long warehouseId,
            DateOnly fromDate,
            DateOnly toDate,
            string createdBy
        )
        {
            var warehouseParam = new SqlParameter("@WarehouseId", warehouseId);
            var fromDateParam = new SqlParameter("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            var toDateParam = new SqlParameter("@ToDate", toDate.ToString("yyyy-MM-dd"));
            var userParam = new SqlParameter("@CreatedBy", createdBy);

            var result = await _context
                .Database.SqlQueryRaw<long>(
                    "EXEC sp_CreateInventorySnapshot @WarehouseId, @FromDate, @ToDate, @CreatedBy",
                    warehouseParam,
                    fromDateParam,
                    toDateParam,
                    userParam
                )
                .ToListAsync();

            return result.FirstOrDefault();
        }
    }
}
