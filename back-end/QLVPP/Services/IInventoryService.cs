using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.DTOs.Result;

namespace QLVPP.Services
{
    public interface IInventoryService
    {
        Task<List<InventorySnapshotRes>> GetListAsync(InventorySnapshotFilterReq filter);
        Task<InventorySnapshotRes?> GetById(long id);
        Task<InventorySnapshotRes> Create(InventorySnapshotReq req);
    }
}
