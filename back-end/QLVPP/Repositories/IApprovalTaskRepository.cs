using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IApprovalTaskRepository : IBaseRepository<ApprovalTask>
    {
        Task<List<ApprovalTask>> GetByProcessIdAndConfigId(long processId, long configId);
    }
}
