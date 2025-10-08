using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;
using QLVPP.Security;

namespace QLVPP.Repositories.Implementations
{
    public class ReturnRepository : BaseRepository<Return>, IReturnRepository
    {
        private readonly AppDbContext _context;
        public ReturnRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Return>> GetAllIsActivated()
        {
            return await _context.Returns
                .Where(r => r.IsActivated == true)
                .ToListAsync();
        }

        public override async Task<Return?> GetById(object id)
        {
            var Id = Convert.ToInt64(id);
            return await _context.Returns
                .Include(r => r.ReturnDetails)
                    .ThenInclude(rd => rd.AssetLoan)
                        .ThenInclude(al => al.DeliveryDetail)
                .FirstOrDefaultAsync(r => r.Id == Id);
        }
    }
}