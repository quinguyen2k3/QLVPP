using Microsoft.EntityFrameworkCore;
using QLVPP.Data;

namespace QLVPP.Repositories.Implementations
{
    public abstract class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : class
    {
        private readonly AppDbContext _context = context;

        public virtual async Task Add(T entity) => await _context.Set<T>().AddAsync(entity);

        public virtual void Delete(T entity) => _context.Set<T>().Remove(entity);

        public virtual async Task<List<T>> GetAll() => await _context.Set<T>()
            .OrderByDescending(e => EF.Property<DateTime>(e, "CreatedDate"))
            .ToListAsync();

        public virtual async Task<T?> GetById(params object[] id) => await _context.Set<T>().FindAsync(id);

        public virtual Task Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
