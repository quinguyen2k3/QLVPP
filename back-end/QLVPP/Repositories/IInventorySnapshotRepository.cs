using QLVPP.DTOs.Request;
using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IInventorySnapshotRepository : IBaseRepository<InventorySnapshot>
    {
        Task<List<InventorySnapshot>> GetListByConditionAsync(InventorySnapshotFilterReq condition);
        Task<InventorySnapshot?> GetOneByConditionAsync(InventorySnapshotFilterReq condition);
        Task<bool> ExistsByConditionAsync(InventorySnapshotFilterReq condition);
        Task<DateOnly?> GetLatestToDateAsync(long? warehouseId = null);
        Task<long> CreateSnapshotProcedure(
            long warehouseId,
            DateOnly fromDate,
            DateOnly toDate,
            string createdBy
        );
    }
}
