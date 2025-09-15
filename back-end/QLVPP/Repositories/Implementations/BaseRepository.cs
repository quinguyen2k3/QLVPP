using Microsoft.EntityFrameworkCore;
using QLVPP.Data;

namespace QLVPP.Repositories.Implementations
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(T entity) => await _context.Set<T>().AddAsync(entity);

        public void Delete(T entity) => _context.Set<T>().Remove(entity);

        public async Task<List<T>> GetAll() => await _context.Set<T>().ToListAsync();

        public async Task<T?> GetById(object id) => await _context.Set<T>().FindAsync(id);

        public async Task Update(T entity) => _context.Set<T>().Update(entity);
    }
}
