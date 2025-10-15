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

        public async Task<List<Order>> GetAllIsActivated()
        {
            return await _context.Orders
                .Where(o => o.IsActivated == true)
                .ToListAsync();
        }

        public override async Task<Order?> GetById(object id)
        {
            var Id = Convert.ToInt64(id);

            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == Id);
        }
    }
}
