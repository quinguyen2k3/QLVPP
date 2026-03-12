using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<string>> GetPermissionsByEmployeeId(long Id)
        {
            return await _context
                .Employees.Where(e => e.Id == Id)
                .SelectMany(e => e.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .ToListAsync();
        }
    }
}
