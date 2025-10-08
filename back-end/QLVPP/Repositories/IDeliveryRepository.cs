using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IDeliveryRepository : IBaseRepository<Delivery>
    {
        Task<List<Delivery>> GetAllIsActivated();
    }
}
