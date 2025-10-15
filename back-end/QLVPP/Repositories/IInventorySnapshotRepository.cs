using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IInventorySnapshotRepository : IBaseRepository<InventorySnapshot>
    {
        Task<List<InventorySnapshot>> GetByWarehouseId(long id);
        Task<bool> ExistsBySnapshotDate(int year, int month);
        Task<InventorySnapshot?> GetLatestByWarehouseId(long id);
    }
}
