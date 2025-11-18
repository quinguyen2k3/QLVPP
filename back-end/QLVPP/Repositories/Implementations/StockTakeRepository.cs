using System;
using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class StockTakeRepository : BaseRepository<StockTake>, IStockTakeRepository
    {
        private readonly AppDbContext _context;

        public StockTakeRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<StockTake>> GetByWarehouseId(long id)
        {
            return await _context
                .StockTakes.Where(s => s.WarehouseId == id && s.IsActivated == true)
                .OrderByDescending(s => s.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<StockTake?> GetById(object id)
        {
            var longId = Convert.ToInt64(id);
            return await _context
                .StockTakes.Include(s => s.Details)
                .FirstOrDefaultAsync(s => s.Id == longId);
        }
    }
}
