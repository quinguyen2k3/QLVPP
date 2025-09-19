using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IWarehouseService
    {
        Task<List<WarehouseRes>> GetAll();
        Task<List<WarehouseRes>> GetAllActived();
        Task<WarehouseRes?> GetById(long id);
        Task<WarehouseRes> Create(WarehouseReq request);
        Task<WarehouseRes?> Update(long id, WarehouseReq request);
    }
}
