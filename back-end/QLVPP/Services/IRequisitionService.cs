using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IRequisitionService
    {
        Task<List<RequisitionRes>> GetAllByMyself();
        Task<List<RequisitionRes>> GetPendingRequisitionsForMe();
        Task<RequisitionRes?> GetById(long id);
        Task<RequisitionRes> Create(RequisitionReq request);
        Task Delegate(DelegateReq request);
        Task Approve(ApproveReq request);
        Task Reject(RejectReq request);
    }
}
