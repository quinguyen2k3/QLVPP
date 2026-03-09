using QLVPP.Security;

namespace QLVPP.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IUnitRepository Unit { get; }
        IEmployeeRepository Employee { get; }
        IRefreshTokenRepository RefreshToken { get; }
        IInvalidTokenRepository InvalidToken { get; }
        IDepartmentRepository Department { get; }
        ISupplierRepository Supplier { get; }
        IWarehouseRepository Warehouse { get; }
        IProductRepository Product { get; }
        IStockInRepository StockIn { get; }
        IInventoryRepository Inventory { get; }
        IStockOutRepository StockOut { get; }
        IInventorySnapshotRepository InventorySnapshot { get; }
        IReportRepository Report { get; }
        IStockTakeRepository StockTake { get; }
        IDepartmentInventoryRepository DepartmentInventory { get; }
        IPositionRepository Position { get; }
        IMaterialRequestRepository MaterialRequest { get; }
        IApprovalLogRepository ApprovalLog { get; }
        Task<int> SaveChanges();
    }
}
