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
    }
}
