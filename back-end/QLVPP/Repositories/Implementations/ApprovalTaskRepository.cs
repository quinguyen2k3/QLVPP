using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class ApprovalTaskRepository : BaseRepository<ApprovalTask>, IApprovalTaskRepository
    {
        private readonly AppDbContext _context;

        public ApprovalTaskRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<ApprovalTask>> GetByConfigId(long configId)
        {
            return await _context
                .ApprovalTasks.Where(at => at.ConfigId == configId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ApprovalTask>> GetPendingByEmployeeId(long employeeId)
        {
            return await _context
                .ApprovalTasks.Include(t => t.Config)
                .ThenInclude(p => p.Requisition)
                .Where(t =>
                    t.Status == RequisitionStatus.Pending
                    && t.AssignedToId == employeeId
                    && (t.DelegateId == null || t.DelegateId == employeeId)
                )
                .ToListAsync();
        }
    }
}
