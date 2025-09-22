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
        Task<int> SaveChanges();
    }
}
