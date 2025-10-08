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

        public async Task<List<Return>> GetAllIsActived()
        {
            return await _context.Returns
                .Where(r => r.IsActived == true)
                .ToListAsync();
        }

        public override async Task<Return?> GetById(params object[] id)
        {
            var orderId = (long)id[0];

            return await _context.Returns
                .Include(r => r.ReturnDetails)
                    .ThenInclude(r => r.AssetLoan)
                        .ThenInclude(r => r.DeliveryDetail)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}