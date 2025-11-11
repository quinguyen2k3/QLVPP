using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface ITransferRepository : IBaseRepository<Transfer>
    {
        Task<List<Transfer>> GetByCreator(string username);
        Task<List<Transfer>> GetPendingByFromWarehouse(long warehouseId);
        Task<List<Transfer>> GetApprovedForWarehouse(long warehouseId);
    }
}
