using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context) : base(context)
        { 
            _context = context;
        }

        public async Task<List<Order>> GetAllIsActived()
        {
            return await _context.Orders
                .Where(o => o.IsActived == true)
                .ToListAsync();
        }

        public override async Task<Order?> GetById(params object[] id)
        {
            var orderId = (long)id[0];

            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}
