using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;
using QLVPP.Security;

namespace QLVPP.Repositories.Implementations
{
    public class ReturnRepository : BaseRepository<Return>, IReturnRepository
    {
        private readonly AppDbContext _context;

        public ReturnRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Return>> GetByWarehouseId(long id)
        {
            return await _context
                .Returns.Where(r => r.WarehouseId == id)
                .OrderByDescending(o => o.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Return>> GetByCreator(string creator)
        {
            return await _context
                .Returns.Where(r => r.CreatedBy == creator)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public override async Task<Return?> GetById(object id)
        {
            var Id = Convert.ToInt64(id);
            return await _context
                .Returns.Include(r => r.ReturnDetails)
                .FirstOrDefaultAsync(r => r.Id == Id);
        }

        public async Task<int> GetTotalReturnedQuantity(long deliveryId, long productId)
        {
            return await _context
                .ReturnDetails.Where(rd =>
                    rd.Return.DeliveryId == deliveryId && rd.ProductId == productId
                )
                .SumAsync(rd => rd.ReturnedQuantity + rd.DamagedQuantity);
        }
    }
}
