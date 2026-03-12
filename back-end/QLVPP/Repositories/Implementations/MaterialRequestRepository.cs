using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.DTOs.Request;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class MaterialRequestRepository
        : BaseRepository<MaterialRequest>,
            IMaterialRequestRepository
    {
        private readonly AppDbContext _context;

        public MaterialRequestRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        private IQueryable<MaterialRequest> BuildQuery(MaterialRequestFilterReq filter)
        {
            var query = _context.MaterialRequests.AsQueryable();

            query = query.Where(x => x.IsActivated == filter.IsActivated);

            if (filter.Statuses != null && filter.Statuses.Any())
            {
                query = query.Where(x => filter.Statuses.Contains(x.Status));
            }

            if (filter.WarehouseId.HasValue)
            {
                query = query.Where(x => x.WarehouseId == filter.WarehouseId);
            }

            if (filter.RequesterId.HasValue)
            {
                query = query.Where(x => x.RequesterId == filter.RequesterId);
            }

            if (filter.ApproverId.HasValue)
            {
                query = query.Where(x => x.ApproverId == filter.ApproverId);
            }

            if (filter.CreatedBy != null)
            {
                query = query.Where(x => x.CreatedBy == filter.CreatedBy);
            }

            if (filter.RequestTypes != null && filter.RequestTypes.Any())
            {
                query = query.Where(x => filter.RequestTypes.Contains(x.Type));
            }

            if (filter.OrderByDesc)
            {
                query = query.OrderByDescending(x => x.CreatedDate);
            }
            else
            {
                query = query.OrderBy(x => x.CreatedDate);
            }

            return query;
        }

        public async Task<List<MaterialRequest>> GetByConditions(MaterialRequestFilterReq filter)
        {
            var query = BuildQuery(filter);
            return await query.ToListAsync();
        }

        public override async Task<MaterialRequest?> GetById(object id)
        {
            var Id = Convert.ToInt64(id);

            return await _context
                .MaterialRequests.Include(o => o.Details)
                .ThenInclude(d => d.Product)
                .ThenInclude(p => p.Unit)
                .FirstOrDefaultAsync(o => o.Id == Id);
        }
    }
}
