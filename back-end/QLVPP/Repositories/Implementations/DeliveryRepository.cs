using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class DeliveryRepository(AppDbContext context) : BaseRepository<Delivery>(context), IDeliveryRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Delivery>> GetAllIsActived()
        {
            return await _context.Deliveries
                .Where(d => d.IsActived == true)
                .ToListAsync();
        }

        public override async Task<Delivery?> GetById(params object[] id)
        {
            var deliveryId = (long)id[0];

            return await _context.Deliveries
                .Include(r => r.DeliveryDetails)
                    .ThenInclude(d => d.Product)
                        .ThenInclude(p => p.Unit)
                .FirstOrDefaultAsync(r => r.Id == deliveryId);
        }
    }
}
