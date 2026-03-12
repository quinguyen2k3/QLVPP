using QLVPP.Data;
using QLVPP.Models;
using QLVPP.Security;

namespace QLVPP.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IUnitRepository Unit { get; private set; }
        public IEmployeeRepository Employee { get; private set; }
        public IRefreshTokenRepository RefreshToken { get; private set; }
        public IInvalidTokenRepository InvalidToken { get; private set; }
        public IDepartmentRepository Department { get; private set; }
        public ISupplierRepository Supplier { get; private set; }
        public IWarehouseRepository Warehouse { get; private set; }
        public IProductRepository Product { get; private set; }
        public IStockInRepository StockIn { get; private set; }
        public IInventoryRepository Inventory { get; private set; }
        public IStockOutRepository StockOut { get; private set; }
        public IInventorySnapshotRepository InventorySnapshot { get; private set; }
        public IReportRepository Report { get; private set; }
        public IStockTakeRepository StockTake { get; private set; }
        public IDepartmentInventoryRepository DepartmentInventory { get; private set; }
        public IPositionRepository Position { get; private set; }
        public IMaterialRequestRepository MaterialRequest { get; private set; }
        public IApprovalLogRepository ApprovalLog { get; private set; }
        public IPermissionRepository Permission { get; private set; }
        public IRoleRepository Role { get; private set; }

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
            Product = new ProductRepository(context);
            StockIn = new StockInRepository(context);
            Inventory = new InventoryRepository(context);
            StockOut = new StockOutRepository(context);
            InventorySnapshot = new InventorySnapshotRepository(context);
            Report = new ReportRepository(context);
            StockTake = new StockTakeRepository(context);
            DepartmentInventory = new DepartmentInventoryRepository(context);
            Warehouse = new WarehouseRepository(context);
            Position = new PositionRepository(context);
            MaterialRequest = new MaterialRequestRepository(context);
            ApprovalLog = new ApprovalLogRepository(context);
            Permission = new PermissionRepository(context);
            Role = new RoleRepository(context);
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
