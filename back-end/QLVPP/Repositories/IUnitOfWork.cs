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
        IOrderRepository Order { get; }
        IInventoryRepository Inventory { get; }
        IDeliveryRepository Delivery { get; }
        IReturnRepository Return { get; }
        IInventorySnapshotRepository InventorySnapshot { get; }
        IReportRepository Report { get; }
        Task<int> SaveChanges();
    }
}
