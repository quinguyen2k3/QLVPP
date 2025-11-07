using QLVPP.DTOs.Projection;
using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IDeliveryRepository : IBaseRepository<Delivery>
    {
        Task<List<Delivery>> GetByWarehouseId(long id);
        Task<List<Delivery>> GetByCreator(string creator);
    }
}
