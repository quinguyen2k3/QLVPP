using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class TransferRepository : BaseRepository<Transfer>, ITransferRepository
    {
        private readonly AppDbContext _context;

        public TransferRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Transfer>> GetByCreator(string username)
        {
            return await _context
                .Transfers.Where(t => t.IsActivated == true && t.CreatedBy == username)
                .OrderByDescending(t => t.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Transfer?> GetById(object id)
        {
            long longId = Convert.ToInt64(id);
            return await _context
                .Transfers.Include(t => t.TransferDetail)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == longId);
        }

        public async Task<List<Transfer>> GetPendingByFromWarehouse(long warehouseId)
        {
            return await _context
                .Transfers.Where(t =>
                    t.FromWarehouseId == warehouseId && t.Status == TransferStatus.Pending
                )
                .OrderBy(t => t.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Transfer>> GetApprovedForWarehouse(long warehouseId)
        {
            return await _context
                .Transfers.Where(t =>
                    t.ToWarehouseId == warehouseId && t.Status == TransferStatus.Approved
                )
                .OrderBy(t => t.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
