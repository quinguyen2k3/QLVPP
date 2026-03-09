using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.DTOs.Result;

namespace QLVPP.Repositories.Implementations
{
    public class DepartmentInventoryRepository : IDepartmentInventoryRepository
    {
        private readonly AppDbContext _context;

        public DepartmentInventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentInventoryResult>> GetInventoryByDepartment(
            long departmentId
        )
        {
            var deptIdParam = new SqlParameter("@DepartmentId", departmentId);

            var inventory = await _context
                .Database.SqlQueryRaw<DepartmentInventoryResult>(
                    "EXEC sp_GetDepartmentInventory @DepartmentId",
                    deptIdParam
                )
                .ToListAsync();

            return inventory;
        }
    }
}
