using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Security
{
    public interface IReturnRepository : IBaseRepository<Return>
    {
        public Task<List<Return>> GetByWarehouseId(long id);
        public Task<int> GetTotalReturnedQuantity(long deliveryId, long productId);
        public Task<List<Return>> GetByCreator(string creator);
    }
}
