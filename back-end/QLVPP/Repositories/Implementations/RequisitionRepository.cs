using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class RequisitionRepository : BaseRepository<Requisition>, IRequisitionRepository
    {
        private readonly AppDbContext _context;

        public RequisitionRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Requisition>> GetByCreator(string creator)
        {
            return await _context
                .Requisitions.Where(r => r.CreatedBy == creator)
                .OrderByDescending(r => r.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Requisition?> GetById(object id)
        {
            var Id = Convert.ToInt64(id);

            return await _context
                .Requisitions.Include(r => r.Config)
                .ThenInclude(r => r.Approvers)
                .Include(r => r.RequisitionDetails)
                .ThenInclude(d => d.Product)
                .ThenInclude(p => p.Unit)
                .FirstOrDefaultAsync(r => r.Id == Id);
        }

        public async Task<List<Requisition>> GetByIds(IEnumerable<long> ids)
        {
            return await _context.Requisitions.Where(r => ids.Contains(r.Id)).ToListAsync();
        }
    }
}
