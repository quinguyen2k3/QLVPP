using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<List<Category>> GetAllIsActivated();
    }
}
