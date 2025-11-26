using Microsoft.EntityFrameworkCore;
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

        public async Task<List<ApprovalTask>> GetByProcessIdAndConfigId(
            long processId,
            long configId
        )
        {
            return await _context
                .ApprovalTasks.Where(at =>
                    at.ApprovalInstanceId == processId && at.ConfigId == configId
                )
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
