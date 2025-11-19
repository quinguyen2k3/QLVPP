using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IRequisitionService
    {
        Task<List<RequisitionRes>> GetAllByMyself();
        Task<RequisitionRes?> GetById(long id);
        Task<RequisitionRes> Create(RequisitionReq request);
        Task<RequisitionRes?> Update(long id, string status);
        Task<RequisitionRes?> Forward(long id, ForwardReq request);
        Task<bool> Delete(long id);
    }
}
