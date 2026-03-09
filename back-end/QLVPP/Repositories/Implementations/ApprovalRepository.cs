using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class ApprovalLogRepository : BaseRepository<ApprovalLog>, IApprovalLogRepository
    {
        public ApprovalLogRepository(AppDbContext context)
            : base(context) { }
    }
}
