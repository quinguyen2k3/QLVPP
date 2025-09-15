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

        public async Task<List<Employee>> GetAllIsActived()
        {
            return await _context.Employees
                                .Where(c => c.IsActived == true)
                                .ToListAsync();
        }

        public async Task<Employee?> GetByAccount(string account)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Account == account);
        }
    }
}
