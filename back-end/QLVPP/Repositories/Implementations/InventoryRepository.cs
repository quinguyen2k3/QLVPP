using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Inventory entity)
        {
            await _context.Set<Inventory>().AddAsync(entity);
        }

        public Task Update(Inventory entity)
        {
            _context.Set<Inventory>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<Inventory?> GetByKey(long warehouseId, long productId)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(i => i.WarehouseId == warehouseId && i.ProductId == productId);
        }

        public async Task<Inventory?> GetByProductId(long productId)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public Task Delete(Inventory entity)
        {
            _context.Inventories.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
