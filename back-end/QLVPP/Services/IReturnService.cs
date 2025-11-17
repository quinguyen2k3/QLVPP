using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IReturnService
    {
        Task<List<ReturnRes>> GetByWarehouse();
        Task<List<ReturnRes>> GetPendingByWarehouse();
        Task<List<ReturnRes>> GetAllByMyself();
        Task<ReturnRes?> GetById(long id);
        Task<ReturnRes> Create(ReturnReq request);
        Task<ReturnRes?> Update(long id, ReturnReq request);
        Task<ReturnRes?> Approve(long id, ReturnReq request);
        Task<ReturnRes?> Return(long id);
        Task<bool> Delete(long id);
    }
}
