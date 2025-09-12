using QLVPP.Data;

namespace QLVPP.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IUnitRepository Unit { get; private set; }

        public readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            Category = new CategoryRepository(context);
            Unit = new UnitRepository(context);
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
