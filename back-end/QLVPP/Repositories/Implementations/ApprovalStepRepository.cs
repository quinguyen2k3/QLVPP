using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class ApprovalStepRepository : BaseRepository<ApprovalStep>, IApprovalStepRepository
    {
        private readonly AppDbContext _context;

        public ApprovalStepRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<ApprovalStep?> GetPendingStepForApproverAsync(
            long requisitionId,
            long approverId
        )
        {
            return await _context
                .ApprovalSteps.Where(s =>
                    s.RequisitionId == requisitionId
                    && s.AssignedToId == approverId
                    && s.Status == RequisitionStatus.Pending
                )
                .OrderBy(s => s.StepOrder)
                .FirstOrDefaultAsync();
        }
    }
}
