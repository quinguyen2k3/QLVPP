using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<List<Product>> GetAllIsActived();
    }
}
