using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllIsActivated()
        {
            return await _context.Employees
                                .Where(e => e.IsActivated == true)
                                .OrderByDescending(e => e.CreatedBy)
                                .ToListAsync();
        }

        public async Task<Employee?> GetByAccount(string account)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Account == account);
        }

        public override async Task<List<Employee>> GetAll()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .OrderByDescending(e => e.CreatedDate) 
                .ToListAsync();
        }
    }
}
