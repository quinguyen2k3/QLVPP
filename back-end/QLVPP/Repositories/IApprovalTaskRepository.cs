using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IApprovalTaskRepository : IBaseRepository<ApprovalTask>
    {
        Task<List<ApprovalTask>> GetByConfigId(long configId);
        Task<List<ApprovalTask>> GetPendingByEmployeeId(long employeeId);
    }
}
