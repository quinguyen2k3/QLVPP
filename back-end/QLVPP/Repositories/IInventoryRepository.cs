using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IInventoryRepository
    {
        Task Add(Inventory entity);
        Task Update(Inventory entity);
        Task<Inventory?> GetByKey(long warehouseId, long productId);
    }
}
