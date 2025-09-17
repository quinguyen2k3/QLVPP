using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IUnitRepository : IBaseRepository<Unit>
    {
        Task<List<Unit>> GetAllIsActived();
    }
}
