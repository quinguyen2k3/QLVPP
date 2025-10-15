using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllIsActivated()
        {
            return await _context.Categories
                                 .Where(c => c.IsActivated == true)
                                 .OrderByDescending(c => c.CreatedBy)
                                 .ToListAsync();
        }
    }
}
