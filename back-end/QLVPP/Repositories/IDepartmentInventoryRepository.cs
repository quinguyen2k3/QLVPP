using QLVPP.DTOs.Result;

namespace QLVPP.Repositories
{
    public interface IDepartmentInventoryRepository
    {
        Task<List<DepartmentInventoryResult>> GetInventoryByDepartment(long departmentId);
    }
}
