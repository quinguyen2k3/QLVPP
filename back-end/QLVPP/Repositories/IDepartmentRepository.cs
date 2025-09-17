using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<List<Department>> GetAllIsActived();
    }
}
