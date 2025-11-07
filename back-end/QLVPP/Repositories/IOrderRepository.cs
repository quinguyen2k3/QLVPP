using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<List<Order>> GetByCreator(string creator);
        Task<List<Order>> GetByWarehouseId(long id);
    }
}
