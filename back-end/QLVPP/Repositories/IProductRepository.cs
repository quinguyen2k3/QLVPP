using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<List<Product>> GetAllIsActivated();
        Task<List<Product>> GetByIds(IEnumerable<long> ids);
    }
}
