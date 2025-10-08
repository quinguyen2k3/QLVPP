using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IReturnService
    {
        Task<List<ReturnRes>> GetAll();
        Task<List<ReturnRes>> GetAllActived();
        Task<ReturnRes?> GetById(long id);
        Task<ReturnRes?> Update(long id, ReturnReq request);
        Task<ReturnRes?> Returned(long id, ReturnReq request);
    }
}