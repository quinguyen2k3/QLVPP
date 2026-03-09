using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IMaterialRequestService
    {
        public Task<List<MaterialRequestRes>> GetByConditions(MaterialRequestFilterReq filter);
        public Task<MaterialRequestRes?> GetById(long Id);
        public Task<MaterialRequestRes> Create(MaterialRequestReq request);
        public Task<MaterialRequestRes> Update(long Id, MaterialRequestReq request);
        public Task<bool> Approve(ApproveReq req);
        public Task<bool> Reject(RejectReq req);
        public Task<bool> Delegate(DelegateReq req);
        public Task<bool> Delete(long Id);
    }
}
