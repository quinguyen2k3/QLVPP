using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<List<Order>> GetAllIsActived();
    }
}
