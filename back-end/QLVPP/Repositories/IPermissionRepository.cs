using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        Task<List<string>> GetPermissionsByEmployeeId(long Id);
    }
}
