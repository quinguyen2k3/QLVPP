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
        IRequisitionRepository Requisition { get; }
        IProductRepository Product { get; }
        IStockInRepository StockIn { get; }
        IInventoryRepository Inventory { get; }
        IStockOutRepository StockOut { get; }
        IReturnRepository Return { get; }
        IInventorySnapshotRepository InventorySnapshot { get; }
        IReportRepository Report { get; }
        ITransferRepository Transfer { get; }
        IStockTakeRepository StockTake { get; }
        IApprovalTaskRepository ApprovalTask { get; }
        IApprovalConfigRepository ApprovalConfig { get; }
        Task<int> SaveChanges();
    }
}
