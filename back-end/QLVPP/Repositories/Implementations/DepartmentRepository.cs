using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Department>> GetAllIsActivated()
        {
            return await _context
                .Departments.Where(c => c.IsActivated == true)
                .OrderByDescending(c => c.CreatedBy)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
