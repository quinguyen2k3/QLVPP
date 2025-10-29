using QLVPP.DTOs.Projection;
using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IDeliveryRepository : IBaseRepository<Delivery>
    {
        Task<List<Delivery>> GetAllIsActivated();
        Task<List<Delivery>> GetByCreator(string creator);
    }
}
