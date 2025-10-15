using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
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

        public async Task<List<InventorySnapshot>> GetByWarehouseId(long id)
        {
            return await _context.InventorySnapshots.Where(i => i.WarehouseId == id).ToListAsync();
        }

        public override async Task<InventorySnapshot?> GetById(object id)
        {
            long longId = Convert.ToInt64(id);

            return await _context
                .InventorySnapshots.Include(s => s.SnapshotDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == longId);
        }

        public async Task<bool> ExistsBySnapshotDate(int year, int month)
        {
            return await _context.InventorySnapshots.AnyAsync(s =>
                s.SnapshotDate.Year == year && s.SnapshotDate.Month == month
            );
        }

        public async Task<InventorySnapshot?> GetLatestByWarehouseId(long id)
        {
            return await _context
                .InventorySnapshots.Where(s => s.WarehouseId == id)
                .OrderByDescending(s => s.SnapshotDate)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
