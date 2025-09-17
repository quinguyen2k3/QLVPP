using QLVPP.Data;

namespace QLVPP.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IUnitRepository Unit { get; private set; }
        public IEmployeeRepository Employee { get; private set; }
        public IRefreshTokenRepository RefreshToken { get; private set; }
        public IInvalidTokenRepository InvalidToken { get; private set; }
        public IDepartmentRepository Department {  get; private set; }
        public ISupplierRepository Supplier {  get; private set; }

        public readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            Category = new CategoryRepository(context);
            Unit = new UnitRepository(context);
            Employee = new EmployeeRepository(context);
            RefreshToken = new RefreshTokenRepository(context);
            InvalidToken = new InvalidTokenRepository(context);
            Department = new DepartmentRepository(context);
            Supplier = new SupplierRepository(context);
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
