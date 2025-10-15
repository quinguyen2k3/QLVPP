using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IInventorySnapshotService
    {
        Task<List<InventorySnapshotRes>> GetByWarehouse();
        Task<InventorySnapshotRes?> GetById(long id);
        Task<InventorySnapshotRes?> Close(long id);
        Task<InventorySnapshotRes> Create();
    }
}
