using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IRequisitionService
    {
        Task<List<RequisitionRes>> GetAll();
        Task<List<RequisitionRes>> GetAllActivated();
        Task<RequisitionRes?> GetById(long id);
        Task<RequisitionRes> Create(RequisitionReq request);
        Task<RequisitionRes?> Update(long id, string status);
        Task<bool> Delete(long id);
    }
}
