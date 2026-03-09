using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<Employee?> GetByAccount(string account);
        Task<List<Employee>> GetAllIsActivated();
        Task<List<Employee>> GetByIds(IEnumerable<long> ids);
        Task<Employee?> GetByPositionNameAndDepartmentId(string name, long departmentId);
    }
}
