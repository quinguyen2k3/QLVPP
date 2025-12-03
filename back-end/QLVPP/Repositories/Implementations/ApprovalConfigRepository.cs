using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class ApprovalConfigRepository
        : BaseRepository<ApprovalConfig>,
            IApprovalConfigRepository
    {
        public ApprovalConfigRepository(AppDbContext context)
            : base(context) { }
    }
}
