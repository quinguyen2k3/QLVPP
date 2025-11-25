using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class ApprovalProcessRepository
        : BaseRepository<ApprovalProcess>,
            IApprovalProcessRepository
    {
        private readonly AppDbContext _context;

        public ApprovalProcessRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
