using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IApprovalStepRepository : IBaseRepository<ApprovalStep>
    {
        Task<ApprovalStep?> GetPendingStepForApproverAsync(long requisitionId, long approverId);
    }
}
