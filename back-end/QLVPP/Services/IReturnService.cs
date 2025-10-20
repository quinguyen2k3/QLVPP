using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IReturnService
    {
        Task<List<ReturnRes>> GetAll();
        Task<List<ReturnRes>> GetAllActivated();
        Task<ReturnRes?> GetById(long id);
        Task<ReturnRes> Create(ReturnReq request);
        Task<ReturnRes?> Update(long id, ReturnReq request);
        Task<ReturnRes?> Returned(long id, ReturnReq request);
        Task<bool> Delete(long id);
    }
}
